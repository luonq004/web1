using Lab2.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Lab2.Models.Data
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MyDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<MyDbContext>>()))
            {
                // 0. Đảm bảo Database và toàn bộ 8 bảng được tạo tự động nếu chưa có
                context.Database.EnsureCreated();

                // -------------------------------------------------------------------------
                // KIỂM TRA ĐỘC LẬP TỪNG BẢNG TRONG CẢ 8 BẢNG:
                // - Bảng nào ĐÃ CÓ dữ liệu -> Bỏ qua, KHÔNG can thiệp, bảo toàn 100% dữ liệu.
                // - Bảng nào CHƯA CÓ dữ liệu -> Tự động nạp dữ liệu mẫu ban đầu.
                // -------------------------------------------------------------------------

                // 1. Kiểm tra bảng Khoa
                if (!context.Khoa.Any())
                {
                    var khoas = new Khoa[]
                    {
                        new Khoa { TenKhoa = "Công nghệ thông tin", MaKhoa = "CNTT" },
                        new Khoa { TenKhoa = "Kinh tế vận tải", MaKhoa = "KTVT" },
                        new Khoa { TenKhoa = "Điện - Điện tử", MaKhoa = "DDT" },
                        new Khoa { TenKhoa = "Công trình giao thông", MaKhoa = "CTGT" }
                    };
                    context.Khoa.AddRange(khoas);
                    context.SaveChanges();
                }

                // 2. Kiểm tra bảng Lop (cần bảng Khoa đã có dữ liệu)
                if (!context.Lops.Any() && context.Khoa.Any())
                {
                    var kCntt = context.Khoa.FirstOrDefault(k => k.MaKhoa == "CNTT") ?? context.Khoa.First();
                    var kKtvt = context.Khoa.FirstOrDefault(k => k.MaKhoa == "KTVT") ?? context.Khoa.First();
                    var kDdt = context.Khoa.FirstOrDefault(k => k.MaKhoa == "DDT") ?? context.Khoa.First();
                    var kCtgt = context.Khoa.FirstOrDefault(k => k.MaKhoa == "CTGT") ?? context.Khoa.First();

                    var lops = new Lop[]
                    {
                        new Lop { TenLop = "CNTT 1 - K62", MaLop = "CNTT1", KhoaId = kCntt.Id },
                        new Lop { TenLop = "CNTT 2 - K62", MaLop = "CNTT2", KhoaId = kCntt.Id },
                        new Lop { TenLop = "Kinh tế 1 - K62", MaLop = "KT1", KhoaId = kKtvt.Id },
                        new Lop { TenLop = "Kinh tế 2 - K62", MaLop = "KT2", KhoaId = kKtvt.Id },
                        new Lop { TenLop = "Điện tử 1 - K62", MaLop = "DT1", KhoaId = kDdt.Id },
                        new Lop { TenLop = "Cầu đường 1 - K62", MaLop = "CD1", KhoaId = kCtgt.Id }
                    };
                    context.Lops.AddRange(lops);
                    context.SaveChanges();
                }

                // 3. Kiểm tra bảng SinhVien (cần bảng Lop đã có dữ liệu)
                if (!context.SinhViens.Any() && context.Lops.Any())
                {
                    var lCntt1 = context.Lops.FirstOrDefault(l => l.MaLop == "CNTT1") ?? context.Lops.First();
                    var lCntt2 = context.Lops.FirstOrDefault(l => l.MaLop == "CNTT2") ?? lCntt1;
                    var lKt1 = context.Lops.FirstOrDefault(l => l.MaLop == "KT1") ?? lCntt1;
                    var lKt2 = context.Lops.FirstOrDefault(l => l.MaLop == "KT2") ?? lCntt1;
                    var lDt1 = context.Lops.FirstOrDefault(l => l.MaLop == "DT1") ?? lCntt1;
                    var lCd1 = context.Lops.FirstOrDefault(l => l.MaLop == "CD1") ?? lCntt1;

                    var sinhViens = new SinhVien[]
                    {
                        new SinhVien { MaSv = "SV001", HoTen = "Nguyễn Hải Nam", NgaySinh = new DateTime(2003, 1, 15), GioiTinh = "Nam", Email = "hainam@utc.edu.vn", LopId = lCntt1.Id },
                        new SinhVien { MaSv = "SV002", HoTen = "Trần Minh Tú", NgaySinh = new DateTime(2003, 5, 20), GioiTinh = "Nữ", Email = "minhtu@utc.edu.vn", LopId = lCntt1.Id },
                        new SinhVien { MaSv = "SV003", HoTen = "Lê Hoàng Phong", NgaySinh = new DateTime(2003, 8, 10), GioiTinh = "Nam", Email = "hoangphong@utc.edu.vn", LopId = lCntt2.Id },
                        new SinhVien { MaSv = "SV004", HoTen = "Phạm Xuân Mai", NgaySinh = new DateTime(2003, 12, 2), GioiTinh = "Nữ", Email = "xuanmai@utc.edu.vn", LopId = lCntt2.Id },
                        new SinhVien { MaSv = "SV005", HoTen = "Đặng Quốc Huy", NgaySinh = new DateTime(2002, 3, 25), GioiTinh = "Nam", Email = "quochuy@utc.edu.vn", LopId = lKt1.Id },
                        new SinhVien { MaSv = "SV006", HoTen = "Vũ Thuỳ Linh", NgaySinh = new DateTime(2003, 7, 18), GioiTinh = "Nữ", Email = "thuylinh@utc.edu.vn", LopId = lKt1.Id },
                        new SinhVien { MaSv = "SV007", HoTen = "Hoàng Văn Tuấn", NgaySinh = new DateTime(2002, 11, 30), GioiTinh = "Nam", Email = "vantuan@utc.edu.vn", LopId = lKt2.Id },
                        new SinhVien { MaSv = "SV008", HoTen = "Bùi Thị Ngọc", NgaySinh = new DateTime(2003, 9, 14), GioiTinh = "Nữ", Email = "thingoc@utc.edu.vn", LopId = lKt2.Id },
                        new SinhVien { MaSv = "SV009", HoTen = "Ngô Trọng Đạt", NgaySinh = new DateTime(2003, 4, 8), GioiTinh = "Nam", Email = "trongdat@utc.edu.vn", LopId = lDt1.Id },
                        new SinhVien { MaSv = "SV010", HoTen = "Dương Khánh Huyền", NgaySinh = new DateTime(2003, 2, 17), GioiTinh = "Nữ", Email = "khanhhuyen@utc.edu.vn", LopId = lDt1.Id },
                        new SinhVien { MaSv = "SV011", HoTen = "Đỗ Đình Khang", NgaySinh = new DateTime(2002, 6, 22), GioiTinh = "Nam", Email = "dinhkhang@utc.edu.vn", LopId = lCd1.Id },
                        new SinhVien { MaSv = "SV012", HoTen = "Trịnh Mai Phương", NgaySinh = new DateTime(2003, 10, 5), GioiTinh = "Nữ", Email = "maiphuong@utc.edu.vn", LopId = lCd1.Id }
                    };
                    context.SinhViens.AddRange(sinhViens);
                    context.SaveChanges();
                }

                // 4. Kiểm tra bảng MonHoc
                if (!context.MonHocs.Any())
                {
                    var monHocs = new MonHoc[]
                    {
                        new MonHoc { MaMon = "IT101", TenMon = "Lập trình C/C++ căn bản", SoTinChi = 3 },
                        new MonHoc { MaMon = "IT201", TenMon = "Cấu trúc dữ liệu & Giải thuật", SoTinChi = 4 },
                        new MonHoc { MaMon = "IT301", TenMon = "Cơ sở dữ liệu", SoTinChi = 3 },
                        new MonHoc { MaMon = "IT401", TenMon = "Lập trình Web nâng cao (.NET)", SoTinChi = 3 },
                        new MonHoc { MaMon = "KT101", TenMon = "Kinh tế vi mô", SoTinChi = 2 },
                        new MonHoc { MaMon = "KT201", TenMon = "Kinh tế vận tải đại cương", SoTinChi = 3 },
                        new MonHoc { MaMon = "CB101", TenMon = "Giải tích 1", SoTinChi = 3 },
                        new MonHoc { MaMon = "CB102", TenMon = "Đại số tuyến tính", SoTinChi = 2 }
                    };
                    context.MonHocs.AddRange(monHocs);
                    context.SaveChanges();
                }

                // 5. Kiểm tra bảng HocKy
                if (!context.HocKys.Any())
                {
                    var hocKys = new HocKy[]
                    {
                        new HocKy { NamHoc = "2023-2024", Ky = 1 },
                        new HocKy { NamHoc = "2023-2024", Ky = 2 },
                        new HocKy { NamHoc = "2023-2024", Ky = 3 },
                        new HocKy { NamHoc = "2024-2025", Ky = 1 },
                        new HocKy { NamHoc = "2024-2025", Ky = 2 },
                        new HocKy { NamHoc = "2024-2025", Ky = 3 }
                    };
                    context.HocKys.AddRange(hocKys);
                    context.SaveChanges();
                }

                // 6. Kiểm tra bảng LopHocPhan (cần bảng MonHoc và HocKy)
                if (!context.LopHocPhans.Any() && context.MonHocs.Any() && context.HocKys.Any())
                {
                    var hk23_1 = context.HocKys.FirstOrDefault(h => h.NamHoc == "2023-2024" && h.Ky == 1) ?? context.HocKys.First();
                    var hk23_2 = context.HocKys.FirstOrDefault(h => h.NamHoc == "2023-2024" && h.Ky == 2) ?? hk23_1;
                    var hk24_1 = context.HocKys.FirstOrDefault(h => h.NamHoc == "2024-2025" && h.Ky == 1) ?? hk23_1;

                    var mIT101 = context.MonHocs.FirstOrDefault(m => m.MaMon == "IT101") ?? context.MonHocs.First();
                    var mIT201 = context.MonHocs.FirstOrDefault(m => m.MaMon == "IT201") ?? mIT101;
                    var mIT301 = context.MonHocs.FirstOrDefault(m => m.MaMon == "IT301") ?? mIT101;
                    var mIT401 = context.MonHocs.FirstOrDefault(m => m.MaMon == "IT401") ?? mIT101;
                    var mKT101 = context.MonHocs.FirstOrDefault(m => m.MaMon == "KT101") ?? mIT101;
                    var mKT201 = context.MonHocs.FirstOrDefault(m => m.MaMon == "KT201") ?? mIT101;
                    var mCB101 = context.MonHocs.FirstOrDefault(m => m.MaMon == "CB101") ?? mIT101;
                    var mCB102 = context.MonHocs.FirstOrDefault(m => m.MaMon == "CB102") ?? mIT101;

                    var lopHocPhans = new LopHocPhan[]
                    {
                        new LopHocPhan { MonHocId = mIT101.Id, HocKyId = hk23_1.Id, GiangVien = "TS. Nguyễn Văn An" },
                        new LopHocPhan { MonHocId = mCB101.Id, HocKyId = hk23_1.Id, GiangVien = "ThS. Trần Thị Bình" },
                        new LopHocPhan { MonHocId = mCB102.Id, HocKyId = hk23_1.Id, GiangVien = "TS. Lê Hoàng Cương" },
                        new LopHocPhan { MonHocId = mKT101.Id, HocKyId = hk23_1.Id, GiangVien = "TS. Hoàng Thu Dung" },

                        new LopHocPhan { MonHocId = mIT201.Id, HocKyId = hk23_2.Id, GiangVien = "PGS.TS. Trần Đình Dũng" },
                        new LopHocPhan { MonHocId = mIT301.Id, HocKyId = hk23_2.Id, GiangVien = "TS. Phạm Thanh Tùng" },
                        new LopHocPhan { MonHocId = mKT201.Id, HocKyId = hk23_2.Id, GiangVien = "TS. Vũ Hải Yến" },
                        new LopHocPhan { MonHocId = mIT101.Id, HocKyId = hk23_2.Id, GiangVien = "ThS. Lê Quang Minh" },

                        new LopHocPhan { MonHocId = mIT401.Id, HocKyId = hk24_1.Id, GiangVien = "TS. Ngô Bảo Quốc" },
                        new LopHocPhan { MonHocId = mIT301.Id, HocKyId = hk24_1.Id, GiangVien = "TS. Phạm Thanh Tùng" },
                        new LopHocPhan { MonHocId = mKT201.Id, HocKyId = hk24_1.Id, GiangVien = "ThS. Đặng Thu Trang" },
                        new LopHocPhan { MonHocId = mCB101.Id, HocKyId = hk24_1.Id, GiangVien = "ThS. Trần Thị Bình" }
                    };
                    context.LopHocPhans.AddRange(lopHocPhans);
                    context.SaveChanges();
                }

                // 7 & 8. Kiểm tra bảng DangKyMon và Diem (cần bảng SinhVien và LopHocPhan)
                if (!context.DangKyMons.Any() && context.SinhViens.Any() && context.LopHocPhans.Any())
                {
                    var svList = context.SinhViens.OrderBy(s => s.Id).ToList();
                    var lhpList = context.LopHocPhans.OrderBy(l => l.Id).ToList();

                    // Tìm các lớp học phần cụ thể
                    var lhpIT101_1 = lhpList.FirstOrDefault(l => l.MonHoc?.MaMon == "IT101") ?? lhpList[0];
                    var lhpCB101_1 = lhpList.FirstOrDefault(l => l.MonHoc?.MaMon == "CB101") ?? lhpList[0];
                    var lhpCB102_1 = lhpList.FirstOrDefault(l => l.MonHoc?.MaMon == "CB102") ?? lhpList[0];
                    var lhpKT101_1 = lhpList.FirstOrDefault(l => l.MonHoc?.MaMon == "KT101") ?? lhpList[0];
                    var lhpIT201_2 = lhpList.FirstOrDefault(l => l.MonHoc?.MaMon == "IT201") ?? lhpList[0];
                    var lhpIT301_2 = lhpList.FirstOrDefault(l => l.MonHoc?.MaMon == "IT301") ?? lhpList[0];
                    var lhpKT201_2 = lhpList.FirstOrDefault(l => l.MonHoc?.MaMon == "KT201") ?? lhpList[0];
                    var lhpIT401_4 = lhpList.FirstOrDefault(l => l.MonHoc?.MaMon == "IT401") ?? lhpList[0];

                    var defaultRegistrations = new (string maSv, LopHocPhan lhp, decimal? qt, decimal? thi)[]
                    {
                        // SV001: Xuất sắc
                        ("SV001", lhpIT101_1, 9.0m, 9.5m),
                        ("SV001", lhpCB101_1, 8.5m, 9.0m),
                        ("SV001", lhpCB102_1, 9.0m, 8.5m),
                        ("SV001", lhpIT201_2, 9.0m, 8.5m),
                        ("SV001", lhpIT301_2, 8.0m, 8.5m),
                        ("SV001", lhpIT401_4, 9.5m, 9.0m),

                        // SV002: Giỏi
                        ("SV002", lhpIT101_1, 8.0m, 8.5m),
                        ("SV002", lhpCB101_1, 7.5m, 8.0m),
                        ("SV002", lhpCB102_1, 8.5m, 8.5m),
                        ("SV002", lhpIT201_2, 8.0m, 8.0m),
                        ("SV002", lhpIT301_2, 8.0m, 8.5m),

                        // SV003: Giỏi
                        ("SV003", lhpIT101_1, 8.0m, 8.0m),
                        ("SV003", lhpCB101_1, 8.5m, 8.0m),
                        ("SV003", lhpCB102_1, 8.0m, 8.0m),
                        ("SV003", lhpIT201_2, 7.5m, 8.0m),
                        ("SV003", lhpIT301_2, 8.5m, 8.5m),

                        // SV004: Khá
                        ("SV004", lhpIT101_1, 7.0m, 7.5m),
                        ("SV004", lhpCB101_1, 6.5m, 7.0m),
                        ("SV004", lhpIT201_2, 7.0m, 7.0m),
                        ("SV004", lhpIT301_2, 7.5m, 7.0m),

                        // SV005: Khá
                        ("SV005", lhpCB101_1, 7.0m, 7.0m),
                        ("SV005", lhpKT101_1, 7.5m, 8.0m),
                        ("SV005", lhpKT201_2, 6.5m, 7.0m),

                        // SV006: Trung bình
                        ("SV006", lhpCB101_1, 5.5m, 6.0m),
                        ("SV006", lhpKT101_1, 6.0m, 6.0m),
                        ("SV006", lhpKT201_2, 6.5m, 6.5m),

                        // SV007: Trung bình
                        ("SV007", lhpCB101_1, 6.0m, 6.0m),
                        ("SV007", lhpKT101_1, 6.5m, 6.0m),
                        ("SV007", lhpKT201_2, 5.5m, 6.0m),

                        // SV008: Yếu
                        ("SV008", lhpCB101_1, 4.5m, 4.0m),
                        ("SV008", lhpKT101_1, 4.0m, 4.5m),
                        ("SV008", lhpKT201_2, 5.0m, 5.0m),

                        // SV009: Yếu
                        ("SV009", lhpIT101_1, 5.0m, 4.0m),
                        ("SV009", lhpCB102_1, 3.0m, 3.5m),

                        // SV010: Khá
                        ("SV010", lhpIT101_1, 7.5m, 7.0m),
                        ("SV010", lhpCB102_1, 8.0m, 7.5m),
                        ("SV010", lhpIT301_2, 7.0m, 6.5m),

                        // SV011: 1 môn có điểm, 1 môn chưa điểm
                        ("SV011", lhpCB101_1, 7.0m, 6.5m),
                        ("SV011", lhpKT101_1, null, null),

                        // SV012: 2 môn chưa có điểm (chờ kiểm thử chức năng nhập điểm)
                        ("SV012", lhpIT101_1, null, null),
                        ("SV012", lhpCB101_1, null, null)
                    };

                    foreach (var reg in defaultRegistrations)
                    {
                        var sv = svList.FirstOrDefault(s => s.MaSv == reg.maSv) ?? svList[0];
                        var dk = new DangKyMon
                        {
                            SinhVienId = sv.Id,
                            LopHocPhanId = reg.lhp.Id
                        };
                        context.DangKyMons.Add(dk);
                        context.SaveChanges();

                        if (reg.qt.HasValue && reg.thi.HasValue)
                        {
                            decimal tk = Math.Round(reg.qt.Value * 0.4m + reg.thi.Value * 0.6m, 1);
                            var diem = new Diem
                            {
                                DangKyId = dk.Id,
                                DiemQuaTrinh = reg.qt.Value,
                                DiemThi = reg.thi.Value,
                                DiemTongKet = tk
                            };
                            context.Diems.Add(diem);
                            context.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}