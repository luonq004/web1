using Lab2.Models;
using Lab2.Models.Data;
using Lab2.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Controllers
{
    public class MonHocController : Controller
    {
        private readonly MyDbContext _context;

        public MonHocController(MyDbContext context)
        {
            _context = context;
        }

        // GET: MonHoc
        public async Task<IActionResult> Index(int? soTinChi, string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSoTinChi"] = soTinChi;

            // Lấy danh sách các số tín chỉ có trong DB để làm bộ lọc
            var tinChiList = await _context.MonHocs
                .Select(m => m.SoTinChi)
                .Distinct()
                .OrderBy(tc => tc)
                .ToListAsync();
            ViewBag.SoTinChiList = new SelectList(tinChiList, soTinChi);

            var monHocs = _context.MonHocs.AsNoTracking();

            // Filter theo Số tín chỉ
            if (soTinChi.HasValue && soTinChi.Value > 0)
            {
                monHocs = monHocs.Where(m => m.SoTinChi == soTinChi.Value);
            }

            // Search theo Tên môn hoặc Mã môn
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                monHocs = monHocs.Where(m => m.TenMon.Contains(searchString) || m.MaMon.Contains(searchString));
            }

            monHocs = monHocs.OrderBy(m => m.Id);

            int pageSize = 5;
            var paginatedMonHocs = await PaginatedList<MonHoc>.CreateAsync(monHocs, pageNumber ?? 1, pageSize);

            return View(paginatedMonHocs);
        }

        // GET: MonHoc/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var monHoc = await _context.MonHocs.FirstOrDefaultAsync(m => m.Id == id);
            if (monHoc == null) return NotFound();

            return View(monHoc);
        }

        // GET: MonHoc/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MonHoc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaMon,TenMon,SoTinChi")] MonHoc monHoc)
        {
            if (ModelState.IsValid)
            {
                if (await _context.MonHocs.AnyAsync(m => m.MaMon == monHoc.MaMon))
                {
                    ModelState.AddModelError("MaMon", "Mã môn học đã tồn tại");
                    return View(monHoc);
                }

                _context.Add(monHoc);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm mới môn học thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(monHoc);
        }

        // GET: MonHoc/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var monHoc = await _context.MonHocs.FindAsync(id);
            if (monHoc == null) return NotFound();

            return View(monHoc);
        }

        // POST: MonHoc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MaMon,TenMon,SoTinChi")] MonHoc monHoc)
        {
            if (id != monHoc.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (await _context.MonHocs.AnyAsync(m => m.MaMon == monHoc.MaMon && m.Id != id))
                {
                    ModelState.AddModelError("MaMon", "Mã môn học đã tồn tại ở môn khác");
                    return View(monHoc);
                }

                try
                {
                    _context.Update(monHoc);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật môn học thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MonHocExists(monHoc.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(monHoc);
        }

        // GET: MonHoc/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var monHoc = await _context.MonHocs.FirstOrDefaultAsync(m => m.Id == id);
            if (monHoc == null) return NotFound();

            return View(monHoc);
        }

        // POST: MonHoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var monHoc = await _context.MonHocs.FindAsync(id);
            if (monHoc != null)
            {
                _context.MonHocs.Remove(monHoc);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã xóa môn học thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MonHocExists(int id)
        {
            return _context.MonHocs.Any(e => e.Id == id);
        }
    }
}
