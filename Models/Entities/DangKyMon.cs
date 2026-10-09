using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab2.Models.Entities
{
    [Table("dang_ky_mon")]
    public class DangKyMon
    {
        public int Id { get; set; }

        [Column("sinh_vien_id")]
        [Required(ErrorMessage = "Sinh viên không được để trống")]
        [Display(Name = "Sinh viên")]
        public int SinhVienId { get; set; }

        [ForeignKey("SinhVienId")]
        public SinhVien? SinhVien { get; set; }

        [Column("lop_hoc_phan_id")]
        [Required(ErrorMessage = "Lớp học phần không được để trống")]
        [Display(Name = "Lớp học phần")]
        public int LopHocPhanId { get; set; }

        [ForeignKey("LopHocPhanId")]
        public LopHocPhan? LopHocPhan { get; set; }

        public Diem? Diem { get; set; }
    }
}
