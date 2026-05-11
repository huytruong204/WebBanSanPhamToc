using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TruongCongHuy_64130895.Models;
namespace TruongCongHuy_64130895.ViewComponents
{

    public class SanPhamSaleViewComponent : ViewComponent
    {
        private readonly ShopOnlineSalesContext context;
        public SanPhamSaleViewComponent(ShopOnlineSalesContext context)
        {
            this.context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var sp = await context.Products
                      .Include(s => s.Category)
                      .Where(r=>r.IsOnSale==true)
                      .OrderBy(s => s.Name)
                      .ToListAsync();
            return View("SanPhamSale",sp);
        }
    }
}
