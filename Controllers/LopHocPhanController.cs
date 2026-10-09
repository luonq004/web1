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
    public class LopHocPhanController : Controller
    {
        private readonly MyDbContext _context;

        public LopHocPhanController(MyDbContext context)
        {
            _context = context;
        }

        // GET: LopHocPhan
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString;

            var myDbContext = _context.LopHocPhans.Include(l => l.HocKy).Include(l => l.MonHoc).AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                myDbContext = myDbContext.Where(l => l.GiangVien.Contains(searchString));
            }

            myDbContext = myDbContext.OrderBy(l => l.Id);

            int pageSize = 5;
            var paginatedList = await PaginatedList<LopHocPhan>.CreateAsync(myDbContext, pageNumber ?? 1, pageSize);
            
            return View(paginatedList);
        }

        // GET: LopHocPhan/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lopHocPhan = await _context.LopHocPhans
                .Include(l => l.HocKy)
                .Include(l => l.MonHoc)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lopHocPhan == null)
            {
                return NotFound();
            }

            return View(lopHocPhan);
        }

        // GET: LopHocPhan/Create
        public IActionResult Create()
        {
            ViewData["HocKyId"] = new SelectList(_context.HocKys, "Id", "NamHoc");
            ViewData["MonHocId"] = new SelectList(_context.MonHocs, "Id", "TenMon");
            return View();
        }

        // POST: LopHocPhan/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MonHocId,HocKyId,GiangVien")] LopHocPhan lopHocPhan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lopHocPhan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HocKyId"] = new SelectList(_context.HocKys, "Id", "NamHoc", lopHocPhan.HocKyId);
            ViewData["MonHocId"] = new SelectList(_context.MonHocs, "Id", "TenMon", lopHocPhan.MonHocId);
            return View(lopHocPhan);
        }

        // GET: LopHocPhan/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lopHocPhan = await _context.LopHocPhans.FindAsync(id);
            if (lopHocPhan == null)
            {
                return NotFound();
            }
            ViewData["HocKyId"] = new SelectList(_context.HocKys, "Id", "NamHoc", lopHocPhan.HocKyId);
            ViewData["MonHocId"] = new SelectList(_context.MonHocs, "Id", "TenMon", lopHocPhan.MonHocId);
            return View(lopHocPhan);
        }

        // POST: LopHocPhan/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MonHocId,HocKyId,GiangVien")] LopHocPhan lopHocPhan)
        {
            if (id != lopHocPhan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lopHocPhan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LopHocPhanExists(lopHocPhan.Id))
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
            ViewData["HocKyId"] = new SelectList(_context.HocKys, "Id", "NamHoc", lopHocPhan.HocKyId);
            ViewData["MonHocId"] = new SelectList(_context.MonHocs, "Id", "TenMon", lopHocPhan.MonHocId);
            return View(lopHocPhan);
        }

        // GET: LopHocPhan/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lopHocPhan = await _context.LopHocPhans
                .Include(l => l.HocKy)
                .Include(l => l.MonHoc)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lopHocPhan == null)
            {
                return NotFound();
            }

            return View(lopHocPhan);
        }

        // POST: LopHocPhan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lopHocPhan = await _context.LopHocPhans.FindAsync(id);
            if (lopHocPhan != null)
            {
                _context.LopHocPhans.Remove(lopHocPhan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LopHocPhanExists(int id)
        {
            return _context.LopHocPhans.Any(e => e.Id == id);
        }
    }
}
