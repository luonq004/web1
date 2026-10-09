using Lab2.Models;
using Lab2.Models.Data;
using Lab2.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Controllers
{
    public class LopController : Controller
    {
        private readonly MyDbContext _context;

        public LopController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Lop
        public async Task<IActionResult> Index(int? khoaId, string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentKhoaId"] = khoaId;

            // Dữ liệu cho Filter Khoa
            ViewBag.KhoaList = new SelectList(await _context.Khoa.OrderBy(k => k.TenKhoa).ToListAsync(), "Id", "TenKhoa", khoaId);

            var lops = _context.Lops
                .Include(l => l.Khoa)
                .Include(l => l.SinhViens)
                .AsNoTracking();

            // Filter theo Khoa
            if (khoaId.HasValue && khoaId.Value > 0)
            {
                lops = lops.Where(l => l.KhoaId == khoaId.Value);
            }

            // Search theo TenLop hoặc MaLop
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                lops = lops.Where(l => l.TenLop.Contains(searchString) || l.MaLop.Contains(searchString));
            }

            lops = lops.OrderBy(l => l.Id);

            int pageSize = 5;
            var paginatedLops = await PaginatedList<Lop>.CreateAsync(lops, pageNumber ?? 1, pageSize);

            return View(paginatedLops);
        }

        // GET: Lop/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var lop = await _context.Lops
                .Include(l => l.Khoa)
                .Include(l => l.SinhViens)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (lop == null) return NotFound();

            return View(lop);
        }

        // GET: Lop/Create
        public async Task<IActionResult> Create()
        {
            ViewData["KhoaId"] = new SelectList(await _context.Khoa.OrderBy(k => k.TenKhoa).ToListAsync(), "Id", "TenKhoa");
            return View();
        }

        // POST: Lop/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenLop,MaLop,KhoaId")] Lop lop)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Lops.AnyAsync(l => l.MaLop == lop.MaLop))
                {
                    ModelState.AddModelError("MaLop", "Mã lớp đã tồn tại");
                    ViewData["KhoaId"] = new SelectList(await _context.Khoa.OrderBy(k => k.TenKhoa).ToListAsync(), "Id", "TenKhoa", lop.KhoaId);
                    return View(lop);
                }

                _context.Add(lop);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm mới lớp thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["KhoaId"] = new SelectList(await _context.Khoa.OrderBy(k => k.TenKhoa).ToListAsync(), "Id", "TenKhoa", lop.KhoaId);
            return View(lop);
        }

        // GET: Lop/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var lop = await _context.Lops.FindAsync(id);
            if (lop == null) return NotFound();

            ViewData["KhoaId"] = new SelectList(await _context.Khoa.OrderBy(k => k.TenKhoa).ToListAsync(), "Id", "TenKhoa", lop.KhoaId);
            return View(lop);
        }

        // POST: Lop/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TenLop,MaLop,KhoaId")] Lop lop)
        {
            if (id != lop.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (await _context.Lops.AnyAsync(l => l.MaLop == lop.MaLop && l.Id != id))
                {
                    ModelState.AddModelError("MaLop", "Mã lớp đã tồn tại ở lớp khác");
                    ViewData["KhoaId"] = new SelectList(await _context.Khoa.OrderBy(k => k.TenKhoa).ToListAsync(), "Id", "TenKhoa", lop.KhoaId);
                    return View(lop);
                }

                try
                {
                    _context.Update(lop);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật lớp thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LopExists(lop.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["KhoaId"] = new SelectList(await _context.Khoa.OrderBy(k => k.TenKhoa).ToListAsync(), "Id", "TenKhoa", lop.KhoaId);
            return View(lop);
        }

        // GET: Lop/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var lop = await _context.Lops
                .Include(l => l.Khoa)
                .Include(l => l.SinhViens)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (lop == null) return NotFound();

            return View(lop);
        }

        // POST: Lop/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lop = await _context.Lops.FindAsync(id);
            if (lop != null)
            {
                _context.Lops.Remove(lop);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã xóa lớp thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LopExists(int id)
        {
            return _context.Lops.Any(e => e.Id == id);
        }
    }
}
