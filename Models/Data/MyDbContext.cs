using Lab2.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Models.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        public DbSet<Khoa> Khoa { get; set; }
        public DbSet<Lop> Lops { get; set; }
        public DbSet<SinhVien> SinhViens { get; set; }
        public DbSet<MonHoc> MonHocs { get; set; }
        public DbSet<HocKy> HocKys { get; set; }
        public DbSet<LopHocPhan> LopHocPhans { get; set; }
        public DbSet<DangKyMon> DangKyMons { get; set; }
        public DbSet<Diem> Diems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Khoa>().ToTable("khoa");
            modelBuilder.Entity<Lop>().ToTable("lop");
            modelBuilder.Entity<SinhVien>().ToTable("sinh_vien");
            modelBuilder.Entity<MonHoc>().ToTable("mon_hoc");
            modelBuilder.Entity<HocKy>().ToTable("hoc_ky");
            modelBuilder.Entity<LopHocPhan>().ToTable("lop_hoc_phan");
            modelBuilder.Entity<DangKyMon>().ToTable("dang_ky_mon");
            modelBuilder.Entity<Diem>().ToTable("diem");

            // Khoa 1 - N Lop
            modelBuilder.Entity<Khoa>()
                .HasMany(k => k.Lops)
                .WithOne(l => l.Khoa)
                .HasForeignKey(l => l.KhoaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Lop 1 - N SinhVien
            modelBuilder.Entity<Lop>()
                .HasMany(l => l.SinhViens)
                .WithOne(sv => sv.Lop)
                .HasForeignKey(sv => sv.LopId)
                .OnDelete(DeleteBehavior.Cascade);

            // MonHoc 1 - N LopHocPhan
            modelBuilder.Entity<MonHoc>()
                .HasMany(m => m.LopHocPhans)
                .WithOne(lhp => lhp.MonHoc)
                .HasForeignKey(lhp => lhp.MonHocId)
                .OnDelete(DeleteBehavior.Cascade);

            // HocKy 1 - N LopHocPhan
            modelBuilder.Entity<HocKy>()
                .HasMany(hk => hk.LopHocPhans)
                .WithOne(lhp => lhp.HocKy)
                .HasForeignKey(lhp => lhp.HocKyId)
                .OnDelete(DeleteBehavior.Cascade);

            // SinhVien 1 - N DangKyMon
            modelBuilder.Entity<SinhVien>()
                .HasMany(sv => sv.DangKyMons)
                .WithOne(dkm => dkm.SinhVien)
                .HasForeignKey(dkm => dkm.SinhVienId)
                .OnDelete(DeleteBehavior.Cascade);

            // LopHocPhan 1 - N DangKyMon
            modelBuilder.Entity<LopHocPhan>()
                .HasMany(lhp => lhp.DangKyMons)
                .WithOne(dkm => dkm.LopHocPhan)
                .HasForeignKey(dkm => dkm.LopHocPhanId)
                .OnDelete(DeleteBehavior.Cascade);

            // DangKyMon 1 - 1 Diem
            modelBuilder.Entity<DangKyMon>()
                .HasOne(dkm => dkm.Diem)
                .WithOne(d => d.DangKyMon)
                .HasForeignKey<Diem>(d => d.DangKyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
