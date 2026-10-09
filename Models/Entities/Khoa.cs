using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab2.Models.Entities
{
    [Table("khoa")]
    public class Khoa
    {
        public int Id { get; set; }

        [Column("ten_khoa")]
        [Required(ErrorMessage = "Tên khoa không được để trống")]
        [StringLength(100, ErrorMessage = "Tên khoa tối đa 100 ký tự")]
        [Display(Name = "Tên khoa")]
        public string TenKhoa { get; set; } = string.Empty;

        [Column("ma_khoa")]
        [Required(ErrorMessage = "Mã khoa không được để trống")]
        [StringLength(20, ErrorMessage = "Mã khoa tối đa 20 ký tự")]
        [Display(Name = "Mã khoa")]
        public string MaKhoa { get; set; } = string.Empty;

        public virtual ICollection<Lop> Lops { get; set; } = new List<Lop>();
    }
}
