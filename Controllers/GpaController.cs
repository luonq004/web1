using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab2.Models;
using Lab2.Models.Data;
using Lab2.Models.Entities;
using Lab2.Models.ViewModels;
using Lab2.Services;

namespace Lab2.Controllers
{
    public class GpaController : Controller
    {
        private readonly MyDbContext _context;

        public GpaController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Gpa
        // Bảng tổng hợp GPA và Xếp loại sinh viên
        public async Task<IActionResult> Index(int? khoaId, int? lopId, int? hocKyId, string? xepLoai, string? searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentKhoaId"] = khoaId;
            ViewData["CurrentLopId"] = lopId;
            ViewData["CurrentHocKyId"] = hocKyId;
            ViewData["CurrentXepLoai"] = xepLoai;

            // Load danh mục cho các Dropdown bộ lọc
            var khoas = await _context.Khoa.OrderBy(k => k.TenKhoa).ToListAsync();
            ViewBag.KhoaList = new SelectList(khoas, "Id", "TenKhoa", khoaId);

            var lopsQuery = _context.Lops.AsQueryable();
            if (khoaId.HasValue && khoaId.Value > 0)
            {
                lopsQuery = lopsQuery.Where(l => l.KhoaId == khoaId.Value);
            }
            var lops = await lopsQuery.OrderBy(l => l.TenLop).ToListAsync();
            ViewBag.LopList = new SelectList(lops, "Id", "TenLop", lopId);

            var hocKys = await _context.HocKys
                .OrderBy(h => h.NamHoc)
                .ThenBy(h => h.Ky)
                .Select(h => new { h.Id, TenHocKy = $"Học kỳ {h.Ky} - {h.NamHoc}" })
                .ToListAsync();
            ViewBag.HocKyList = new SelectList(hocKys, "Id", "TenHocKy", hocKyId);

            var xepLoaiOptions = new List<string> { "Xuất sắc", "Giỏi", "Khá", "Trung bình", "Yếu" };
            ViewBag.XepLoaiList = new SelectList(xepLoaiOptions, xepLoai);

            // Query sinh viên
            var svQuery = _context.SinhViens
                .Include(s => s.Lop)
                    .ThenInclude(l => l!.Khoa)
                .Include(s => s.DangKyMons)
                    .ThenInclude(dk => dk.LopHocPhan)
                        .ThenInclude(lhp => lhp!.MonHoc)
                .Include(s => s.DangKyMons)
                    .ThenInclude(dk => dk.LopHocPhan)
                        .ThenInclude(lhp => lhp!.HocKy)
                .Include(s => s.DangKyMons)
                    .ThenInclude(dk => dk.Diem)
                .AsNoTracking();

            if (khoaId.HasValue && khoaId.Value > 0)
            {
                svQuery = svQuery.Where(s => s.Lop != null && s.Lop.KhoaId == khoaId.Value);
            }

            if (lopId.HasValue && lopId.Value > 0)
            {
                svQuery = svQuery.Where(s => s.LopId == lopId.Value);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                svQuery = svQuery.Where(s => s.HoTen.Contains(searchString) || s.MaSv.Contains(searchString));
            }

            var sinhViens = await svQuery.OrderBy(s => s.Id).ToListAsync();

            // Tính toán GPA & Xếp loại cho từng sinh viên
            var summaryList = new List<SinhVienGpaSummaryViewModel>();

            foreach (var sv in sinhViens)
            {
                var dangKys = sv.DangKyMons.AsEnumerable();

                // Nếu lọc theo học kỳ cụ thể
                if (hocKyId.HasValue && hocKyId.Value > 0)
                {
                    dangKys = dangKys.Where(dk => dk.LopHocPhan != null && dk.LopHocPhan.HocKyId == hocKyId.Value);
                }

                var listDk = dangKys.ToList();
                int tongTcDangKy = 0;
                int tongTcTichLuy = 0;
                int soMonCoDiem = 0;
                int soMonChuaCoDiem = 0;

                var diemThang10List = new List<(decimal diem, int tinChi)>();
                var diemThang4List = new List<(decimal diem, int tinChi)>();

                foreach (var dk in listDk)
                {
                    int tc = dk.LopHocPhan?.MonHoc?.SoTinChi ?? 0;
                    tongTcDangKy += tc;

                    decimal? diemTk = dk.Diem?.DiemTongKet;
                    if (!diemTk.HasValue && dk.Diem != null && dk.Diem.DiemQuaTrinh.HasValue && dk.Diem.DiemThi.HasValue)
                    {
                        diemTk = GpaCalculator.TinhDiemTongKet(dk.Diem.DiemQuaTrinh, dk.Diem.DiemThi);
                    }

                    if (diemTk.HasValue && tc > 0)
                    {
                        soMonCoDiem++;
                        diemThang10List.Add((diemTk.Value, tc));

                        var diem4 = GpaCalculator.QuyDoiThang4(diemTk.Value) ?? 0;
                        diemThang4List.Add((diem4, tc));

                        // Môn đạt nếu điểm thang 10 >= 4.0
                        if (diemTk.Value >= 4.0m)
                        {
                            tongTcTichLuy += tc;
                        }
                    }
                    else
                    {
                        soMonChuaCoDiem++;
                    }
                }

                var gpa10 = GpaCalculator.TinhGpa(diemThang10List);
                var gpa4 = GpaCalculator.TinhGpa(diemThang4List);
                var xepLoaiHocLuc = GpaCalculator.XepLoai(gpa4);

                summaryList.Add(new SinhVienGpaSummaryViewModel
                {
                    SinhVienId = sv.Id,
                    MaSv = sv.MaSv,
                    HoTen = sv.HoTen,
                    TenLop = sv.Lop?.TenLop ?? "Chưa xếp lớp",
                    TenKhoa = sv.Lop?.Khoa?.TenKhoa ?? "Chưa rõ",
                    TongTinChiDangKy = tongTcDangKy,
                    TongTinChiTichLuy = tongTcTichLuy,
                    GpaThang10 = gpa10,
                    GpaThang4 = gpa4,
                    XepLoai = xepLoaiHocLuc,
                    SoMonDaCoDiem = soMonCoDiem,
                    SoMonChuaCoDiem = soMonChuaCoDiem
                });
            }

            // Thống kê nhanh toàn bộ tập kết quả (trước khi lọc xếp loại)
            ViewBag.CountTotal = summaryList.Count;
            ViewBag.CountXuatSac = summaryList.Count(s => s.XepLoai == "Xuất sắc");
            ViewBag.CountGioi = summaryList.Count(s => s.XepLoai == "Giỏi");
            ViewBag.CountKha = summaryList.Count(s => s.XepLoai == "Khá");
            ViewBag.CountTrungBinh = summaryList.Count(s => s.XepLoai == "Trung bình");
            ViewBag.CountYeu = summaryList.Count(s => s.XepLoai == "Yếu");

            // Lọc theo Xếp loại nếu người dùng chọn
            if (!string.IsNullOrEmpty(xepLoai))
            {
                summaryList = summaryList.Where(s => s.XepLoai == xepLoai).ToList();
            }

            // Phân trang
            int pageSize = 10;
            int pageIdx = pageNumber ?? 1;
            if (pageIdx < 1) pageIdx = 1;
            int totalCount = summaryList.Count;
            var pagedItems = summaryList.Skip((pageIdx - 1) * pageSize).Take(pageSize).ToList();

            var paginatedModel = new PaginatedList<SinhVienGpaSummaryViewModel>(pagedItems, totalCount, pageIdx, pageSize);

            return View(paginatedModel);
        }

        // GET: Gpa/Details/5
        // Xem bảng điểm chi tiết theo từng học kỳ và tích lũy của 1 sinh viên
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var sv = await _context.SinhViens
                .Include(s => s.Lop)
                    .ThenInclude(l => l!.Khoa)
                .Include(s => s.DangKyMons)
                    .ThenInclude(dk => dk.LopHocPhan)
                        .ThenInclude(lhp => lhp!.MonHoc)
                .Include(s => s.DangKyMons)
                    .ThenInclude(dk => dk.LopHocPhan)
                        .ThenInclude(lhp => lhp!.HocKy)
                .Include(s => s.DangKyMons)
                    .ThenInclude(dk => dk.Diem)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sv == null) return NotFound();

            var model = new BangDiemChiTietViewModel
            {
                SinhVienId = sv.Id,
                MaSv = sv.MaSv,
                HoTen = sv.HoTen,
                NgaySinh = sv.NgaySinh,
                GioiTinh = sv.GioiTinh,
                Email = sv.Email,
                TenLop = sv.Lop?.TenLop ?? "Chưa rõ",
                TenKhoa = sv.Lop?.Khoa?.TenKhoa ?? "Chưa rõ"
            };

            // Nhóm đăng ký môn theo Học kỳ
            var groupedByHocKy = sv.DangKyMons
                .Where(dk => dk.LopHocPhan != null && dk.LopHocPhan.HocKy != null)
                .GroupBy(dk => dk.LopHocPhan!.HocKy!)
                .OrderBy(g => g.Key.NamHoc)
                .ThenBy(g => g.Key.Ky);

            var toanKhoaDiem10 = new List<(decimal diem, int tinChi)>();
            var toanKhoaDiem4 = new List<(decimal diem, int tinChi)>();
            int tongTcDangKyToanKhoa = 0;
            int tongTcDatToanKhoa = 0;

            foreach (var group in groupedByHocKy)
            {
                var hk = group.Key;
                var hkVm = new HocKyDiemViewModel
                {
                    HocKyId = hk.Id,
                    NamHoc = hk.NamHoc,
                    Ky = hk.Ky
                };

                var hkDiem10 = new List<(decimal diem, int tinChi)>();
                var hkDiem4 = new List<(decimal diem, int tinChi)>();

                foreach (var dk in group)
                {
                    var mh = dk.LopHocPhan?.MonHoc;
                    int tc = mh?.SoTinChi ?? 0;
                    tongTcDangKyToanKhoa += tc;

                    decimal? diemTk = dk.Diem?.DiemTongKet;
                    if (!diemTk.HasValue && dk.Diem != null && dk.Diem.DiemQuaTrinh.HasValue && dk.Diem.DiemThi.HasValue)
                    {
                        diemTk = GpaCalculator.TinhDiemTongKet(dk.Diem.DiemQuaTrinh, dk.Diem.DiemThi);
                    }

                    decimal? diemHe4 = GpaCalculator.QuyDoiThang4(diemTk);
                    string diemChu = GpaCalculator.QuyDoiDiemChu(diemTk);

                    hkVm.DanhSachMon.Add(new MonHocDiemViewModel
                    {
                        LopHocPhanId = dk.LopHocPhanId,
                        MaMon = mh?.MaMon ?? "N/A",
                        TenMon = mh?.TenMon ?? "N/A",
                        SoTinChi = tc,
                        GiangVien = dk.LopHocPhan?.GiangVien ?? "Chưa phân công",
                        DiemQuaTrinh = dk.Diem?.DiemQuaTrinh,
                        DiemThi = dk.Diem?.DiemThi,
                        DiemTongKet = diemTk,
                        DiemHe4 = diemHe4,
                        DiemChu = diemChu
                    });

                    if (diemTk.HasValue && tc > 0)
                    {
                        hkDiem10.Add((diemTk.Value, tc));
                        toanKhoaDiem10.Add((diemTk.Value, tc));

                        if (diemHe4.HasValue)
                        {
                            hkDiem4.Add((diemHe4.Value, tc));
                            toanKhoaDiem4.Add((diemHe4.Value, tc));
                        }

                        if (diemTk.Value >= 4.0m)
                        {
                            tongTcDatToanKhoa += tc;
                        }
                    }
                }

                hkVm.GpaHocKyThang10 = GpaCalculator.TinhGpa(hkDiem10);
                hkVm.GpaHocKyThang4 = GpaCalculator.TinhGpa(hkDiem4);
                hkVm.XepLoaiHocKy = GpaCalculator.XepLoai(hkVm.GpaHocKyThang4);

                model.DanhSachHocKy.Add(hkVm);
            }

            model.TongTinChiDangKy = tongTcDangKyToanKhoa;
            model.TongTinChiTichLuy = tongTcDatToanKhoa;
            model.GpaTichLuyThang10 = GpaCalculator.TinhGpa(toanKhoaDiem10);
            model.GpaTichLuyThang4 = GpaCalculator.TinhGpa(toanKhoaDiem4);
            model.XepLoaiTichLuy = GpaCalculator.XepLoai(model.GpaTichLuyThang4);

            return View(model);
        }
    }
}

