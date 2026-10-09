using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab2.Models.Entities
{
    [Table("lop")]
    public class Lop
    {
        public int Id { get; set; }

        [Column("ten_lop")]
        [Required(ErrorMessage = "Tên lớp không được để trống")]
        [StringLength(100, ErrorMessage = "Tên lớp tối đa 100 ký tự")]
        [Display(Name = "Tên lớp")]
        public string TenLop { get; set; } = string.Empty;

        [Column("ma_lop")]
        [Required(ErrorMessage = "Mã lớp không được để trống")]
        [StringLength(20, ErrorMessage = "Mã lớp tối đa 20 ký tự")]
        [Display(Name = "Mã lớp")]
        public string MaLop { get; set; } = string.Empty;

        [Column("khoa_id")]
        [Required(ErrorMessage = "Vui lòng chọn khoa")]
        [Display(Name = "Khoa")]
        public int KhoaId { get; set; }

        public virtual Khoa? Khoa { get; set; }

        public virtual ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();
    }
}
