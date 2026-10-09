using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab2.Models.Entities
{
    [Table("mon_hoc")]
    public class MonHoc
    {
        public int Id { get; set; }

        [Column("ma_mon")]
        [Required(ErrorMessage = "Mã môn không được để trống")]
        [StringLength(20, ErrorMessage = "Mã môn tối đa 20 ký tự")]
        [Display(Name = "Mã môn học")]
        public string MaMon { get; set; } = string.Empty;

        [Column("ten_mon")]
        [Required(ErrorMessage = "Tên môn không được để trống")]
        [StringLength(100, ErrorMessage = "Tên môn tối đa 100 ký tự")]
        [Display(Name = "Tên môn học")]
        public string TenMon { get; set; } = string.Empty;

        [Column("so_tin_chi")]
        [Required(ErrorMessage = "Số tín chỉ không được để trống")]
        [Range(1, 10, ErrorMessage = "Số tín chỉ phải từ 1 đến 10")]
        [Display(Name = "Số tín chỉ")]
        public int SoTinChi { get; set; } = 3;

        public ICollection<LopHocPhan> LopHocPhans { get; set; } = new List<LopHocPhan>();
    }
}
