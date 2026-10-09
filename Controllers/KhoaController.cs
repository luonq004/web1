using Lab2.Models;
using Lab2.Models.Data;
using Lab2.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Controllers
{
    public class KhoaController : Controller
    {
        private readonly MyDbContext _context;

        public KhoaController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Khoa
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString;

            var khoas = _context.Khoa
                .Include(k => k.Lops)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                khoas = khoas.Where(k => k.TenKhoa.Contains(searchString) || k.MaKhoa.Contains(searchString));
            }

            khoas = khoas.OrderBy(k => k.Id);

            int pageSize = 5;
            var paginatedKhoas = await PaginatedList<Khoa>.CreateAsync(khoas, pageNumber ?? 1, pageSize);

            return View(paginatedKhoas);
        }

        // GET: Khoa/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var khoa = await _context.Khoa
                .Include(k => k.Lops)
                .ThenInclude(l => l.SinhViens)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (khoa == null) return NotFound();

            return View(khoa);
        }

        // GET: Khoa/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Khoa/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenKhoa,MaKhoa")] Khoa khoa)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra mã khoa trùng
                if (await _context.Khoa.AnyAsync(k => k.MaKhoa == khoa.MaKhoa))
                {
                    ModelState.AddModelError("MaKhoa", "Mã khoa đã tồn tại");
                    return View(khoa);
                }

                _context.Add(khoa);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm mới khoa thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(khoa);
        }

        // GET: Khoa/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var khoa = await _context.Khoa.FindAsync(id);
            if (khoa == null) return NotFound();

            return View(khoa);
        }

        // POST: Khoa/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TenKhoa,MaKhoa")] Khoa khoa)
        {
            if (id != khoa.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (await _context.Khoa.AnyAsync(k => k.MaKhoa == khoa.MaKhoa && k.Id != id))
                {
                    ModelState.AddModelError("MaKhoa", "Mã khoa đã tồn tại ở khoa khác");
                    return View(khoa);
                }

                try
                {
                    _context.Update(khoa);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật khoa thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhoaExists(khoa.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(khoa);
        }

        // GET: Khoa/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var khoa = await _context.Khoa
                .Include(k => k.Lops)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (khoa == null) return NotFound();

            return View(khoa);
        }

        // POST: Khoa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khoa = await _context.Khoa.FindAsync(id);
            if (khoa != null)
            {
                _context.Khoa.Remove(khoa);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã xóa khoa thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool KhoaExists(int id)
        {
            return _context.Khoa.Any(e => e.Id == id);
        }
    }
}
