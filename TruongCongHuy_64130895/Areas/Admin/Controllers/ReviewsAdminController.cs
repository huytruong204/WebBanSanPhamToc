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
    public class ReviewsAdminController : Controller
    {
        private readonly ShopOnlineSalesContext _context;

        public ReviewsAdminController(ShopOnlineSalesContext context)
        {
            _context = context;
        }

        // GET: Admin/ReviewsAdmin
        [Authorize(Roles = "Quản Trị Viên, Nhân Viên")]
        public async Task<IActionResult> Index(int page = 1, int pagesize = 5)
        {
            var shopOnlineSalesContext = _context.Reviews.Include(r => r.Order).Include(r => r.Product);

            return View(await shopOnlineSalesContext.ToPagedListAsync(page, pagesize));
        }

        // GET: Admin/ReviewsAdmin/Details/5
        public async Task<IActionResult> Details(int? reviewId)
        {
            if (reviewId == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Order)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(m => m.ReviewId == reviewId);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }
        // GET: Admin/ReviewsAdmin/Delete/5
        [Authorize(Roles = "Quản Trị Viên")]
        [HttpGet]
        public async Task<IActionResult> Delete(int? reviewId)
        {
            if (reviewId == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Order)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(m => m.ReviewId == reviewId);
            if (review == null)
            {
                return NotFound();
            }
            return View(review);
        }

        // POST: Admin/ReviewsAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);

            if (review == null)
            {
                Console.WriteLine($"Không tìm thấy Review với ID: {reviewId}");
                return NotFound();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
