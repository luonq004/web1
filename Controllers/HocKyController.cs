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
    public class HocKyController : Controller
    {
        private readonly MyDbContext _context;

        public HocKyController(MyDbContext context)
        {
            _context = context;
        }

        // GET: HocKy
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString;

            var hocKys = _context.HocKys.AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                hocKys = hocKys.Where(h => h.NamHoc.Contains(searchString));
            }

            hocKys = hocKys.OrderBy(h => h.Id);

            int pageSize = 5;
            var paginatedHocKys = await PaginatedList<HocKy>.CreateAsync(hocKys, pageNumber ?? 1, pageSize);

            return View(paginatedHocKys);
        }

        // GET: HocKy/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocKy = await _context.HocKys
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hocKy == null)
            {
                return NotFound();
            }

            return View(hocKy);
        }

        // GET: HocKy/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HocKy/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NamHoc,Ky")] HocKy hocKy)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hocKy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hocKy);
        }

        // GET: HocKy/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocKy = await _context.HocKys.FindAsync(id);
            if (hocKy == null)
            {
                return NotFound();
            }
            return View(hocKy);
        }

        // POST: HocKy/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NamHoc,Ky")] HocKy hocKy)
        {
            if (id != hocKy.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hocKy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HocKyExists(hocKy.Id))
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
            return View(hocKy);
        }

        // GET: HocKy/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocKy = await _context.HocKys
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hocKy == null)
            {
                return NotFound();
            }

            return View(hocKy);
        }

        // POST: HocKy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hocKy = await _context.HocKys.FindAsync(id);
            if (hocKy != null)
            {
                _context.HocKys.Remove(hocKy);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HocKyExists(int id)
        {
            return _context.HocKys.Any(e => e.Id == id);
        }
    }
}
