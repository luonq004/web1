using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab2.Models.Data;
using Lab2.Models.Entities;
using Lab2.Models;

namespace Lab2.Controllers
{
    public class DiemController : Controller
    {
        private readonly MyDbContext _context;

        public DiemController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Diem
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString;

            var myDbContext = _context.Diems.Include(d => d.DangKyMon).ThenInclude(dk => dk.SinhVien).Include(d => d.DangKyMon).ThenInclude(dk => dk.LopHocPhan).ThenInclude(l => l.MonHoc).AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                myDbContext = myDbContext.Where(d => d.DangKyMon.SinhVien.HoTen.Contains(searchString) || d.DangKyMon.SinhVien.MaSv.Contains(searchString));
            }

            myDbContext = myDbContext.OrderBy(d => d.Id);

            int pageSize = 5;
            var paginatedList = await PaginatedList<Diem>.CreateAsync(myDbContext, pageNumber ?? 1, pageSize);
            
            return View(paginatedList);
        }

        // GET: Diem/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diem = await _context.Diems
                .Include(d => d.DangKyMon).ThenInclude(dk => dk.SinhVien)
                .Include(d => d.DangKyMon).ThenInclude(dk => dk.LopHocPhan).ThenInclude(l => l.MonHoc)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diem == null)
            {
                return NotFound();
            }

            return View(diem);
        }

        // GET: Diem/Create
        public IActionResult Create()
        {
            ViewData["DangKyId"] = new SelectList(_context.DangKyMons.Include(d => d.SinhVien).Include(d => d.LopHocPhan).ThenInclude(l => l.MonHoc).Select(d => new { Id = d.Id, Name = d.SinhVien.HoTen + " - " + d.LopHocPhan.MonHoc.TenMon }), "Id", "Name");
            return View();
        }

        // POST: Diem/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DangKyId,DiemQuaTrinh,DiemThi,DiemTongKet")] Diem diem)
        {
            if (ModelState.IsValid)
            {
                bool exists = await _context.Diems.AnyAsync(d => d.DangKyId == diem.DangKyId);
                if (exists)
                {
                    ModelState.AddModelError("DangKyId", "Bản ghi đăng ký này đã được nhập điểm. Vui lòng chuyển sang chức năng Cập nhật thay vì Thêm mới.");
                }
                else
                {
                    if (diem.DiemQuaTrinh.HasValue && diem.DiemThi.HasValue)
                    {
                        diem.DiemTongKet = Math.Round(diem.DiemQuaTrinh.Value * 0.4m + diem.DiemThi.Value * 0.6m, 1);
                    }
                    else
                    {
                        diem.DiemTongKet = null;
                    }
                    _context.Add(diem);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["DangKyId"] = new SelectList(_context.DangKyMons.Include(d => d.SinhVien).Include(d => d.LopHocPhan).ThenInclude(l => l.MonHoc).Select(d => new { Id = d.Id, Name = d.SinhVien.HoTen + " - " + d.LopHocPhan.MonHoc.TenMon }), "Id", "Name", diem.DangKyId);
            return View(diem);
        }

        // GET: Diem/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diem = await _context.Diems.FindAsync(id);
            if (diem == null)
            {
                return NotFound();
            }
            ViewData["DangKyId"] = new SelectList(_context.DangKyMons.Include(d => d.SinhVien).Include(d => d.LopHocPhan).ThenInclude(l => l.MonHoc).Select(d => new { Id = d.Id, Name = d.SinhVien.HoTen + " - " + d.LopHocPhan.MonHoc.TenMon }), "Id", "Name", diem.DangKyId);
            return View(diem);
        }

        // POST: Diem/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DangKyId,DiemQuaTrinh,DiemThi,DiemTongKet")] Diem diem)
        {
            if (id != diem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (diem.DiemQuaTrinh.HasValue && diem.DiemThi.HasValue)
                    {
                        diem.DiemTongKet = Math.Round(diem.DiemQuaTrinh.Value * 0.4m + diem.DiemThi.Value * 0.6m, 1);
                    }
                    else
                    {
                        diem.DiemTongKet = null;
                    }
                    _context.Update(diem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiemExists(diem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DangKyId"] = new SelectList(_context.DangKyMons.Include(d => d.SinhVien).Include(d => d.LopHocPhan).ThenInclude(l => l.MonHoc).Select(d => new { Id = d.Id, Name = d.SinhVien.HoTen + " - " + d.LopHocPhan.MonHoc.TenMon }), "Id", "Name", diem.DangKyId);
            return View(diem);
        }

        // GET: Diem/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diem = await _context.Diems
                .Include(d => d.DangKyMon).ThenInclude(dk => dk.SinhVien)
                .Include(d => d.DangKyMon).ThenInclude(dk => dk.LopHocPhan).ThenInclude(l => l.MonHoc)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diem == null)
            {
                return NotFound();
            }

            return View(diem);
        }

        // POST: Diem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diem = await _context.Diems.FindAsync(id);
            if (diem != null)
            {
                _context.Diems.Remove(diem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiemExists(int id)
        {
            return _context.Diems.Any(e => e.Id == id);
        }
    }
}
