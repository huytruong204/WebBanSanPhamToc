using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TruongCongHuy_64130895.Models;

namespace TruongCongHuy_64130895.ViewComponents
{
    public class SanPhamBanChayViewComponent: ViewComponent
    {
        private readonly ShopOnlineSalesContext context;
        public SanPhamBanChayViewComponent(ShopOnlineSalesContext context)
        {
            this.context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var sp = await context.Products
                      .Include(s => s.Category)
                      .OrderBy(s => s.Name)
                      .ToListAsync();
            return View("SanPhamBanChay", sp);
        }
    }
}
