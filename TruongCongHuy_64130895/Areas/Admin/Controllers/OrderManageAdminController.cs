using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using TruongCongHuy_64130895.Areas.Admin.Models;
using TruongCongHuy_64130895.Models;
using X.PagedList;
namespace TruongCongHuy_64130895.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Quản Trị Viên, Nhân Viên")]
    public class OrderManageAdminController : Controller
    {
        private readonly ShopOnlineSalesContext context;
        private readonly UserManager<Customers> userManager;
        public OrderManageAdminController(ShopOnlineSalesContext context, UserManager<Customers> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        public async Task<IActionResult> ManageOrders(string status = "Tất cả", int page=1, int pagesize=7)
        {
            var orders = context.Orders.AsQueryable();
            if (status != "Tất cả")
            {
                orders = orders.Where(o => o.Status == status);
            }
            var orderList = await orders.ToListAsync();

            var orderDetail = new List<OrderViewModel>();
            foreach (var order in orderList)
            {
                var customer = await userManager.FindByIdAsync(order.UserId);
                var customerName = customer?.FullName;

                orderDetail.Add(new OrderViewModel
                {
                    OrderId = order.Id,
                    OrderDate = order.OrderDate,
                    Status = order.Status,
                    CustomerName = customerName,
                    TotalPrice = order.TotalAmount

                });
            }
            var pagedResult = orderDetail.OrderByDescending(o => o.OrderDate).ToPagedList(page, pagesize);
            var statusList = new List<string> { "Tất cả", "Chờ xác nhận", "Đang xử lý", "Đang giao", "Đã giao", "Đã Hủy", "Hoàn tiền" };
            ViewBag.Status = new SelectList(statusList, status);
            ViewBag.St = status;
            return View(pagedResult);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveOrder(int orderId)
        {
            var order = await context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại.");
            }

            order.Status = "Đang xử lý";
            await context.SaveChangesAsync();

            TempData["Message"] = $"Đơn hàng #{orderId} đã được duyệt.";
            return RedirectToAction("ManageOrders");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var order = await context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại.");
            }

            order.Status = "Đã Hủy";
            await context.SaveChangesAsync();

            TempData["Message"] = $"Đơn hàng #{orderId} đã bị hủy.";
            return RedirectToAction("ManageOrders");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsShipped(int orderId)
        {
            var order = await context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại.");
            }

            // Chuyển trạng thái đơn hàng sang Shipped
            order.Status = "Đang giao";
            await context.SaveChangesAsync();

            TempData["Message"] = $"Đơn hàng #{orderId} đã được đánh dấu là đang giao hàng.";
            return RedirectToAction("ManageOrders");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> MarkAsDelivered(int orderId)
        {
            var order = await context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại.");
            }

            // Chuyển trạng thái đơn hàng sang Delivered
            order.Status = "Đã giao";
            await context.SaveChangesAsync();

            TempData["Message"] = $"Đơn hàng #{orderId} đã được giao thành công.";
            return RedirectToAction("ManageOrders");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> MarkAsRefunded(int orderId)
        {
            var order = await context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại.");
            }

            order.Status = "Hoàn tiền";
            await context.SaveChangesAsync();

            TempData["Message"] = $"Đơn hàng #{orderId} đã được hoàn tiền.";
            return RedirectToAction("ManageOrders");
        }
        [HttpGet]
        public async Task<IActionResult> OrdersDetail(int? id)
        {
            if (id == null)
            {
                return NotFound("Order ID không được cung cấp.");
            }

            var orderDetails = await context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .Where(od => od.OrderId == id)
                .ToListAsync();

            if (!orderDetails.Any())
            {
                return NotFound("Không tìm thấy chi tiết đơn hàng.");
            }

            var payment = await context.Payments
                .FirstOrDefaultAsync(p => p.OrderId == id);

            ViewBag.PaymentMethod = payment?.PaymentMethod ?? "Chưa thanh toán";

            return View(orderDetails);
        }
    }

}
