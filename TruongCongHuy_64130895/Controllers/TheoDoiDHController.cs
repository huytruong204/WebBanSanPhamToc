using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TruongCongHuy_64130895.Models;
using X.PagedList;
namespace TruongCongHuy_64130895.Controllers
{
    public class TheoDoiDHController : Controller
    {
        private readonly ShopOnlineSalesContext context;
        public TheoDoiDHController(ShopOnlineSalesContext context)
        {
            this.context = context;
        }
        public async Task<IActionResult> Index(int page=1, int pagesize=5)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var orders = await context.Orders
                .Where(o => o.UserId == userId) 
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToPagedListAsync(page, pagesize);
            ViewData["ExistingReviews"] = await context.Reviews.Where(r => r.UserId == userId).ToListAsync();
            return View(orders);
        }
        [HttpGet]
        public IActionResult DanhGiaSp(int? id, int? orderID)
        {
            if (id == null || orderID == null)
            {
                TempData["Error"] = "Sản phẩm hoặc đơn hàng không hợp lệ.";
                return RedirectToAction("Index");
            }
            ViewData["ProductId"] = id;
            ViewData["OrderId"] = orderID;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DanhGiaSP(int? id, int? orderID, Review review)
        {
            var sp = await context.Products.FindAsync(id);

            var order = await context.Orders.FindAsync(orderID);
            review.ProductId = sp.Id;
            review.UserId = order.UserId!;
            review.OrderId = order.Id;
            review.CreatedAt = DateTime.Now;
            context.Add(review);
            await context.SaveChangesAsync();

            TempData["Success"] = "Đánh giá của bạn đã được ghi nhận.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int? orderID)
        {
            var order = await context.Orders.FindAsync(orderID);
            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại.");
            }
            order.Status = "Đã Hủy";
            await context.SaveChangesAsync();

            TempData["Success"] = $"Đơn hàng #{orderID} đã bị hủy.";
            return RedirectToAction("Index");
        }


    }
}
