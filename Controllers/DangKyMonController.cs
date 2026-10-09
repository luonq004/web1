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
    public class DangKyMonController : Controller
    {
        private readonly MyDbContext _context;

        public DangKyMonController(MyDbContext context)
        {
            _context = context;
        }

        // GET: DangKyMon
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString;

            var myDbContext = _context.DangKyMons.Include(d => d.LopHocPhan).ThenInclude(l => l.MonHoc).Include(d => d.SinhVien).AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                myDbContext = myDbContext.Where(d => d.SinhVien.HoTen.Contains(searchString) || d.SinhVien.MaSv.Contains(searchString));
            }

            myDbContext = myDbContext.OrderBy(d => d.Id);

            int pageSize = 5;
            var paginatedList = await PaginatedList<DangKyMon>.CreateAsync(myDbContext, pageNumber ?? 1, pageSize);
            
            return View(paginatedList);
        }

        // GET: DangKyMon/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dangKyMon = await _context.DangKyMons
                .Include(d => d.SinhVien)
                .Include(d => d.LopHocPhan).ThenInclude(l => l.MonHoc)
                .Include(d => d.LopHocPhan).ThenInclude(l => l.HocKy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dangKyMon == null)
            {
                return NotFound();
            }

            return View(dangKyMon);
        }

        // GET: DangKyMon/Create
        public IActionResult Create()
        {
            ViewData["LopHocPhanId"] = new SelectList(_context.LopHocPhans.Include(l => l.MonHoc).Select(l => new { Id = l.Id, Name = l.MonHoc.TenMon + " - GV: " + l.GiangVien }), "Id", "Name");
            ViewData["SinhVienId"] = new SelectList(_context.SinhViens, "Id", "HoTen");
            return View();
        }

        // POST: DangKyMon/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SinhVienId,LopHocPhanId")] DangKyMon dangKyMon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dangKyMon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LopHocPhanId"] = new SelectList(_context.LopHocPhans.Include(l => l.MonHoc).Select(l => new { Id = l.Id, Name = l.MonHoc.TenMon + " - GV: " + l.GiangVien }), "Id", "Name", dangKyMon.LopHocPhanId);
            ViewData["SinhVienId"] = new SelectList(_context.SinhViens, "Id", "HoTen", dangKyMon.SinhVienId);
            return View(dangKyMon);
        }

        // GET: DangKyMon/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dangKyMon = await _context.DangKyMons.FindAsync(id);
            if (dangKyMon == null)
            {
                return NotFound();
            }
            ViewData["LopHocPhanId"] = new SelectList(_context.LopHocPhans.Include(l => l.MonHoc).Select(l => new { Id = l.Id, Name = l.MonHoc.TenMon + " - GV: " + l.GiangVien }), "Id", "Name", dangKyMon.LopHocPhanId);
            ViewData["SinhVienId"] = new SelectList(_context.SinhViens, "Id", "HoTen", dangKyMon.SinhVienId);
            return View(dangKyMon);
        }

        // POST: DangKyMon/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SinhVienId,LopHocPhanId")] DangKyMon dangKyMon)
        {
            if (id != dangKyMon.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dangKyMon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DangKyMonExists(dangKyMon.Id))
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
            ViewData["LopHocPhanId"] = new SelectList(_context.LopHocPhans.Include(l => l.MonHoc).Select(l => new { Id = l.Id, Name = l.MonHoc.TenMon + " - GV: " + l.GiangVien }), "Id", "Name", dangKyMon.LopHocPhanId);
            ViewData["SinhVienId"] = new SelectList(_context.SinhViens, "Id", "HoTen", dangKyMon.SinhVienId);
            return View(dangKyMon);
        }

        // GET: DangKyMon/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dangKyMon = await _context.DangKyMons
                .Include(d => d.SinhVien)
                .Include(d => d.LopHocPhan).ThenInclude(l => l.MonHoc)
                .Include(d => d.LopHocPhan).ThenInclude(l => l.HocKy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dangKyMon == null)
            {
                return NotFound();
            }

            return View(dangKyMon);
        }

        // POST: DangKyMon/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dangKyMon = await _context.DangKyMons.FindAsync(id);
            if (dangKyMon != null)
            {
                _context.DangKyMons.Remove(dangKyMon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DangKyMonExists(int id)
        {
            return _context.DangKyMons.Any(e => e.Id == id);
        }
    }
}
