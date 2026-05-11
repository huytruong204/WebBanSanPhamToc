using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TruongCongHuy_64130895.Models;

namespace TruongCongHuy_64130895.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuAdminController : Controller
    {
        private readonly ShopOnlineSalesContext _context;

        public MenuAdminController(ShopOnlineSalesContext context)
        {
            _context = context;
        }

        // GET: Admin/MenuAdmin
        [Authorize(Roles = "Quản Trị Viên, Nhân Viên")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Menus.ToListAsync());
        }


        // GET: Admin/MenuAdmin/Create
        [Authorize(Roles = "Quản Trị Viên")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/MenuAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ParentId,Title,MenuUrl,MenuIndex,IsVisible")] MENU mENU)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mENU);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mENU);
        }

        // GET: Admin/MenuAdmin/Edit/5
        [Authorize(Roles = "Quản Trị Viên")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mENU = await _context.Menus.FindAsync(id);
            if (mENU == null)
            {
                return NotFound();
            }
            return View(mENU);
        }

        // POST: Admin/MenuAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ParentId,Title,MenuUrl,MenuIndex,IsVisible")] MENU mENU)
        {
            if (id != mENU.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mENU);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MENUExists(mENU.Id))
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
            return View(mENU);
        }

        // GET: Admin/MenuAdmin/Delete/5
        [Authorize(Roles = "Quản Trị Viên")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mENU = await _context.Menus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mENU == null)
            {
                return NotFound();
            }

            return View(mENU);
        }

        // POST: Admin/MenuAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mENU = await _context.Menus.FindAsync(id);
            if (mENU != null)
            {
                _context.Menus.Remove(mENU);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MENUExists(int id)
        {
            return _context.Menus.Any(e => e.Id == id);
        }
    }
}
