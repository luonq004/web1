using Lab2.Models;
using Lab2.Models.Data;
using Lab2.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Controllers
{
    public class SinhVienController : Controller
    {
        private readonly MyDbContext _context;

        public SinhVienController(MyDbContext context)
        {
            _context = context;
        }

        // GET: SinhVien
        public async Task<IActionResult> Index(int? lopId, int? khoaId, string? gioiTinh, string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentLopId"] = lopId;
            ViewData["CurrentKhoaId"] = khoaId;
            ViewData["CurrentGioiTinh"] = gioiTinh;

            // Load danh sách Khoa và Lớp cho bộ lọc Filter
            var khoas = await _context.Khoa.OrderBy(k => k.TenKhoa).ToListAsync();
            ViewBag.KhoaList = new SelectList(khoas, "Id", "TenKhoa", khoaId);

            var lopsQuery = _context.Lops.AsQueryable();
            if (khoaId.HasValue && khoaId.Value > 0)
            {
                lopsQuery = lopsQuery.Where(l => l.KhoaId == khoaId.Value);
            }
            var lops = await lopsQuery.OrderBy(l => l.TenLop).ToListAsync();
            ViewBag.LopList = new SelectList(lops, "Id", "TenLop", lopId);

            var sinhViens = _context.SinhViens
                .Include(s => s.Lop)
                .ThenInclude(l => l!.Khoa)
                .AsNoTracking();

            // Filter theo Khoa
            if (khoaId.HasValue && khoaId.Value > 0)
            {
                sinhViens = sinhViens.Where(s => s.Lop!.KhoaId == khoaId.Value);
            }

            // Filter theo Lớp
            if (lopId.HasValue && lopId.Value > 0)
            {
                sinhViens = sinhViens.Where(s => s.LopId == lopId.Value);
            }

            // Filter theo Giới tính
            if (!string.IsNullOrEmpty(gioiTinh))
            {
                sinhViens = sinhViens.Where(s => s.GioiTinh == gioiTinh);
            }

            // Search từ khóa (Mã SV, Họ tên, Email)
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                sinhViens = sinhViens.Where(s => s.HoTen.Contains(searchString) 
                                              || s.MaSv.Contains(searchString) 
                                              || s.Email.Contains(searchString));
            }

            sinhViens = sinhViens.OrderBy(s => s.Id);

            int pageSize = 5;
            var paginatedStudents = await PaginatedList<SinhVien>.CreateAsync(sinhViens, pageNumber ?? 1, pageSize);

            return View(paginatedStudents);
        }

        // GET: SinhVien/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var sinhVien = await _context.SinhViens
                .Include(s => s.Lop)
                .ThenInclude(l => l!.Khoa)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sinhVien == null) return NotFound();

            return View(sinhVien);
        }

        // GET: SinhVien/Create
        public async Task<IActionResult> Create()
        {
            await PopulateLopDropDownList();
            var sv = new SinhVien
            {
                NgaySinh = new DateTime(2003, 1, 1),
                GioiTinh = "Nam"
            };
            return View(sv);
        }

        // POST: SinhVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSv,HoTen,NgaySinh,GioiTinh,Email,LopId")] SinhVien sinhVien)
        {
            if (ModelState.IsValid)
            {
                if (await _context.SinhViens.AnyAsync(s => s.MaSv == sinhVien.MaSv))
                {
                    ModelState.AddModelError("MaSv", "Mã sinh viên đã tồn tại");
                    await PopulateLopDropDownList(sinhVien.LopId);
                    return View(sinhVien);
                }

                _context.Add(sinhVien);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm mới sinh viên thành công!";
                return RedirectToAction(nameof(Index));
            }
            await PopulateLopDropDownList(sinhVien.LopId);
            return View(sinhVien);
        }

        // GET: SinhVien/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien == null) return NotFound();

            await PopulateLopDropDownList(sinhVien.LopId);
            return View(sinhVien);
        }

        // POST: SinhVien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaSv,HoTen,NgaySinh,GioiTinh,Email,LopId")] SinhVien sinhVien)
        {
            if (id != sinhVien.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (await _context.SinhViens.AnyAsync(s => s.MaSv == sinhVien.MaSv && s.Id != id))
                {
                    ModelState.AddModelError("MaSv", "Mã sinh viên đã tồn tại ở sinh viên khác");
                    await PopulateLopDropDownList(sinhVien.LopId);
                    return View(sinhVien);
                }

                try
                {
                    _context.Update(sinhVien);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật sinh viên thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SinhVienExists(sinhVien.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            await PopulateLopDropDownList(sinhVien.LopId);
            return View(sinhVien);
        }

        // GET: SinhVien/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var sinhVien = await _context.SinhViens
                .Include(s => s.Lop)
                .ThenInclude(l => l!.Khoa)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sinhVien == null) return NotFound();

            return View(sinhVien);
        }

        // POST: SinhVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien != null)
            {
                _context.SinhViens.Remove(sinhVien);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã xóa sinh viên thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateLopDropDownList(object? selectedLop = null)
        {
            var lops = await _context.Lops.Include(l => l.Khoa).OrderBy(l => l.TenLop).ToListAsync();
            ViewData["LopId"] = new SelectList(lops.Select(l => new {
                l.Id,
                DisplayName = $"{l.TenLop} (Khoa {l.Khoa?.TenKhoa})"
            }), "Id", "DisplayName", selectedLop);
        }

        private bool SinhVienExists(int id)
        {
            return _context.SinhViens.Any(e => e.Id == id);
        }
    }
}
