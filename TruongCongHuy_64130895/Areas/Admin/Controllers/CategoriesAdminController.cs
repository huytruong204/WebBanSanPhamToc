using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TruongCongHuy_64130895.Models;
using X.PagedList;

namespace TruongCongHuy_64130895.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class CategoriesAdminController : Controller
    {
        private readonly ShopOnlineSalesContext _context;

        public CategoriesAdminController(ShopOnlineSalesContext context)
        {
            _context = context;
        }

        // GET: Admin/CategoriesAdmin
        [Authorize(Roles = "Quản Trị Viên, Nhân Viên")]
        public async Task<IActionResult> Index(string? TimKiem="", int page=1, int pagesize=5)
        {
            var categories = _context.Categories.ToList().AsQueryable();
            if(TimKiem != null)
                categories = categories.Where(t=>t.Name.Contains(TimKiem));
            ViewBag.TimKiem = TimKiem;
            return View(await categories.ToPagedListAsync(page, pagesize));
        }

        // GET: Admin/CategoriesAdmin/Create
        [Authorize(Roles = "Quản Trị Viên")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/CategoriesAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,CreatedDate")] Category category)
        {
            if (ModelState.IsValid)
            {
                category.CreatedDate = DateTime.Now;
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/CategoriesAdmin/Edit/5
        [Authorize(Roles = "Quản Trị Viên")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/CategoriesAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,CreatedDate")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            return View(category);
        }

        // GET: Admin/CategoriesAdmin/Delete/5
        [Authorize(Roles = "Quản Trị Viên")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/CategoriesAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
