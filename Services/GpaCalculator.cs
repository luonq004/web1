using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab2.Services
{
    public static class GpaCalculator
    {
        /// <summary>
        /// Tính điểm tổng kết môn học thang điểm 10 từ điểm quá trình (40%) và điểm thi (60%).
        /// </summary>
        public static decimal? TinhDiemTongKet(decimal? diemQuaTrinh, decimal? diemThi)
        {
            if (!diemQuaTrinh.HasValue || !diemThi.HasValue)
                return null;
            return Math.Round(diemQuaTrinh.Value * 0.4m + diemThi.Value * 0.6m, 1);
        }

        /// <summary>
        /// Quy đổi điểm thang 10 sang thang điểm 4 theo quy chế đào tạo tín chỉ.
        /// </summary>
        public static decimal? QuyDoiThang4(decimal? diemThang10)
        {
            if (!diemThang10.HasValue) return null;
            var d = diemThang10.Value;

            if (d >= 8.5m) return 4.0m;
            if (d >= 8.0m) return 3.5m;
            if (d >= 7.0m) return 3.0m;
            if (d >= 6.5m) return 2.5m;
            if (d >= 5.5m) return 2.0m;
            if (d >= 5.0m) return 1.5m;
            if (d >= 4.0m) return 1.0m;
            return 0.0m;
        }

        /// <summary>
        /// Quy đổi điểm thang 10 sang điểm chữ (A, B+, B, C+, C, D+, D, F).
        /// </summary>
        public static string QuyDoiDiemChu(decimal? diemThang10)
        {
            if (!diemThang10.HasValue) return "-";
            var d = diemThang10.Value;

            if (d >= 8.5m) return "A";
            if (d >= 8.0m) return "B+";
            if (d >= 7.0m) return "B";
            if (d >= 6.5m) return "C+";
            if (d >= 5.5m) return "C";
            if (d >= 5.0m) return "D+";
            if (d >= 4.0m) return "D";
            return "F";
        }

        /// <summary>
        /// Tính điểm trung bình GPA có trọng số theo số tín chỉ: GPA = sum(Điểm * Số tín chỉ) / sum(Số tín chỉ).
        /// </summary>
        public static decimal? TinhGpa(IEnumerable<(decimal diem, int tinChi)> danhSach)
        {
            var list = danhSach.Where(x => x.tinChi > 0).ToList();
            if (!list.Any()) return null;

            int tongTinChi = list.Sum(x => x.tinChi);
            if (tongTinChi == 0) return null;

            decimal tongDiemNhanTinChi = list.Sum(x => x.diem * x.tinChi);
            return Math.Round(tongDiemNhanTinChi / tongTinChi, 2);
        }

        /// <summary>
        /// Xếp loại học lực theo GPA thang 4 chuẩn quy chế đại học:
        /// - Xuất sắc: 3.60 - 4.00
        /// - Giỏi: 3.20 - 3.59
        /// - Khá: 2.50 - 3.19
        /// - Trung bình: 2.00 - 2.49
        /// - Yếu: &lt; 2.00
        /// </summary>
        public static string XepLoai(decimal? gpaThang4)
        {
            if (!gpaThang4.HasValue) return "Chưa xếp loại";
            var gpa = gpaThang4.Value;

            if (gpa >= 3.60m) return "Xuất sắc";
            if (gpa >= 3.20m) return "Giỏi";
            if (gpa >= 2.50m) return "Khá";
            if (gpa >= 2.00m) return "Trung bình";
            return "Yếu";
        }

        /// <summary>
        /// Xếp loại học lực theo GPA thang 10:
        /// - Xuất sắc: 9.0 - 10.0
        /// - Giỏi: 8.0 - 8.9
        /// - Khá: 7.0 - 7.9
        /// - Trung bình: 5.0 - 6.9
        /// - Yếu: &lt; 5.0
        /// </summary>
        public static string XepLoaiThang10(decimal? gpaThang10)
        {
            if (!gpaThang10.HasValue) return "Chưa xếp loại";
            var gpa = gpaThang10.Value;

            if (gpa >= 9.0m) return "Xuất sắc";
            if (gpa >= 8.0m) return "Giỏi";
            if (gpa >= 7.0m) return "Khá";
            if (gpa >= 5.0m) return "Trung bình";
            return "Yếu";
        }

        /// <summary>
        /// Lớp CSS cho Badge hiển thị xếp loại
        /// </summary>
        public static string LayBadgeClass(string xepLoai)
        {
            return xepLoai switch
            {
                "Xuất sắc" => "bg-danger text-white",
                "Giỏi" => "bg-success text-white",
                "Khá" => "bg-primary text-white",
                "Trung bình" => "bg-warning text-dark",
                "Yếu" => "bg-secondary text-white",
                _ => "bg-light text-muted border"
            };
        }
    }
}

