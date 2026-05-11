using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TruongCongHuy_64130895.Models;

namespace TruongCongHuy_64130895.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Quản Trị Viên, Nhân Viên")]
    public class HomeAdminController : Controller
    {
        private readonly ShopOnlineSalesContext context;
        public HomeAdminController(ShopOnlineSalesContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            ViewBag.Sale = context.Products.Count(t => t.IsOnSale == true);
            ViewBag.Sold = context.Orders.Count(t => t.Status == "Đã giao");
            ViewBag.Order = context.Orders.Count();
            ViewBag.Total = context.Orders.Where(t => t.Status == "Đã giao").Sum(s => s.TotalAmount);
            var revenueData = context.Orders
            .Where(o => o.Status == "Đã giao") 
            .GroupBy(o => o.OrderDate.Date) 
            .Select (g => new
            {
                Date = g.Key,
                Revenue = g.Sum(o => o.TotalAmount)
            })
            .OrderBy(g => g.Date) 
            .ToList();

            ViewBag.ChartData = revenueData;

            return View();
        }
        [Route("/AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
