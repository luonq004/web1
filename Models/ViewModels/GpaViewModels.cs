using System;
using System.Collections.Generic;

namespace Lab2.Models.ViewModels
{
    // Thông tin điểm của một môn học
    public class MonHocDiemViewModel
    {
        public int LopHocPhanId { get; set; }
        public string MaMon { get; set; } = string.Empty;
        public string TenMon { get; set; } = string.Empty;
        public int SoTinChi { get; set; }
        public string GiangVien { get; set; } = string.Empty;
        public decimal? DiemQuaTrinh { get; set; }
        public decimal? DiemThi { get; set; }
        public decimal? DiemTongKet { get; set; } // Thang 10
        public decimal? DiemHe4 { get; set; }     // Thang 4
        public string DiemChu { get; set; } = string.Empty; // A, B+, B, C, D, F
        public bool DaDat => (DiemTongKet ?? 0) >= 4.0m;
    }

    // Bảng điểm theo từng Học kỳ
    public class HocKyDiemViewModel
    {
        public int HocKyId { get; set; }
        public string NamHoc { get; set; } = string.Empty;
        public int Ky { get; set; }
        public string TenHocKy => $"Học kỳ {Ky} - Năm học {NamHoc}";

        public List<MonHocDiemViewModel> DanhSachMon { get; set; } = new();

        public int TongTinChiDangKy => DanhSachMon.Sum(m => m.SoTinChi);
        public int TongTinChiDat => DanhSachMon.Where(m => m.DaDat).Sum(m => m.SoTinChi);

        // GPA học kỳ thang 10 & thang 4
        public decimal? GpaHocKyThang10 { get; set; }
        public decimal? GpaHocKyThang4 { get; set; }
        public string XepLoaiHocKy { get; set; } = string.Empty;
    }

    // Bảng điểm chi tiết của một Sinh viên (toàn khóa học)
    public class BangDiemChiTietViewModel
    {
        public int SinhVienId { get; set; }
        public string MaSv { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public DateTime NgaySinh { get; set; }
        public string GioiTinh { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string TenLop { get; set; } = string.Empty;
        public string TenKhoa { get; set; } = string.Empty;

        public List<HocKyDiemViewModel> DanhSachHocKy { get; set; } = new();

        // Tích lũy toàn khóa
        public int TongTinChiDangKy { get; set; }
        public int TongTinChiTichLuy { get; set; }
        public decimal? GpaTichLuyThang10 { get; set; }
        public decimal? GpaTichLuyThang4 { get; set; }
        public string XepLoaiTichLuy { get; set; } = string.Empty;
    }

    // Dòng tóm tắt trong danh sách xếp loại toàn trường / khoa / lớp
    public class SinhVienGpaSummaryViewModel
    {
        public int SinhVienId { get; set; }
        public string MaSv { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string TenLop { get; set; } = string.Empty;
        public string TenKhoa { get; set; } = string.Empty;

        // Số tín chỉ
        public int TongTinChiDangKy { get; set; }
        public int TongTinChiTichLuy { get; set; }

        // GPA và Xếp loại (có thể là tích lũy hoặc theo học kỳ được chọn lọc)
        public decimal? GpaThang10 { get; set; }
        public decimal? GpaThang4 { get; set; }
        public string XepLoai { get; set; } = string.Empty;

        public int SoMonDaCoDiem { get; set; }
        public int SoMonChuaCoDiem { get; set; }
    }
}

