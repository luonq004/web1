using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab2.Models.Entities
{
    [Table("sinh_vien")]
    public class SinhVien
    {
        public int Id { get; set; }

        [Column("ma_sv")]
        [Required(ErrorMessage = "Mã sinh viên không được để trống")]
        [StringLength(20, ErrorMessage = "Mã sinh viên tối đa 20 ký tự")]
        [Display(Name = "Mã SV")]
        public string MaSv { get; set; } = string.Empty;

        [Column("ho_ten")]
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Column("ngay_sinh")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime NgaySinh { get; set; }

        [Column("gioi_tinh")]
        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        [StringLength(10)]
        [Display(Name = "Giới tính")]
        public string GioiTinh { get; set; } = "Nam";

        [Column("email")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Column("lop_id")]
        [Required(ErrorMessage = "Vui lòng chọn lớp")]
        [Display(Name = "Lớp")]
        public int LopId { get; set; }

        public virtual Lop? Lop { get; set; }

        public ICollection<DangKyMon> DangKyMons { get; set; } = new List<DangKyMon>();
    }
}
