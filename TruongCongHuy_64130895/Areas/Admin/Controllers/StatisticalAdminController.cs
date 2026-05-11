using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruongCongHuy_64130895.Models;

namespace TruongCongHuy_64130895.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Quản Trị Viên, Nhân Viên")]
    public class StatisticalAdminController : Controller
    {
        private readonly ShopOnlineSalesContext context;

        public StatisticalAdminController(ShopOnlineSalesContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public IActionResult RevenueStatistics()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RevenueStatistics(DateTime startDate, DateTime endDate)
        {
            var orders = context.Orders
                .Where(o => o.Status == "Đã giao" && o.OrderDate.Date >= startDate.Date && o.OrderDate.Date <= endDate.Date)
                .ToList();

            var totalRevenue = orders.Sum(o => o.TotalAmount);
            var totalOrders = orders.Count;
            var revenueByDay = orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalRevenue = g.Sum(o => o.TotalAmount),
                    TotalOrders = g.Count()
                })
                .OrderBy(g => g.Date)
                .ToList();

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.RevenueByDay = revenueByDay;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            return View();
        }
    }
}
