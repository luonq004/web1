using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab2.Models.Entities
{
    [Table("hoc_ky")]
    public class HocKy
    {
        public int Id { get; set; }

        [Column("nam_hoc")]
        [Required(ErrorMessage = "Năm học không được để trống")]
        [StringLength(20, ErrorMessage = "Năm học tối đa 20 ký tự")]
        [Display(Name = "Năm học")]
        public string NamHoc { get; set; } = string.Empty;

        [Column("ky")]
        [Required(ErrorMessage = "Học kỳ không được để trống")]
        [Range(1, 3, ErrorMessage = "Học kỳ phải là 1, 2, hoặc 3")]
        [Display(Name = "Học kỳ")]
        public int Ky { get; set; }

        public ICollection<LopHocPhan> LopHocPhans { get; set; } = new List<LopHocPhan>();
    }
}
