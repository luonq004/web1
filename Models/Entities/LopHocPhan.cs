using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab2.Models.Entities
{
    [Table("lop_hoc_phan")]
    public class LopHocPhan
    {
        public int Id { get; set; }

        [Column("mon_hoc_id")]
        [Required(ErrorMessage = "Môn học không được để trống")]
        [Display(Name = "Môn học")]
        public int MonHocId { get; set; }

        [ForeignKey("MonHocId")]
        public MonHoc? MonHoc { get; set; }

        [Column("hoc_ky_id")]
        [Required(ErrorMessage = "Học kỳ không được để trống")]
        [Display(Name = "Học kỳ")]
        public int HocKyId { get; set; }

        [ForeignKey("HocKyId")]
        public HocKy? HocKy { get; set; }

        [Column("giang_vien")]
        [Required(ErrorMessage = "Giảng viên không được để trống")]
        [StringLength(100, ErrorMessage = "Tên giảng viên tối đa 100 ký tự")]
        [Display(Name = "Giảng viên")]
        public string GiangVien { get; set; } = string.Empty;

        public ICollection<DangKyMon> DangKyMons { get; set; } = new List<DangKyMon>();
    }
}
