using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab2.Models.Entities
{
    [Table("diem")]
    public class Diem
    {
        public int Id { get; set; }

        [Column("dang_ky_id")]
        [Required(ErrorMessage = "Đăng ký môn không được để trống")]
        [Display(Name = "Đăng ký môn")]
        public int DangKyId { get; set; }

        [ForeignKey("DangKyId")]
        public DangKyMon? DangKyMon { get; set; }

        [Column("diem_qua_trinh")]
        [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
        [Display(Name = "Điểm quá trình")]
        public decimal? DiemQuaTrinh { get; set; }

        [Column("diem_thi")]
        [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
        [Display(Name = "Điểm thi")]
        public decimal? DiemThi { get; set; }

        [Column("diem_tong_ket")]
        [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
        [Display(Name = "Điểm tổng kết")]
        public decimal? DiemTongKet { get; set; }
    }
}
