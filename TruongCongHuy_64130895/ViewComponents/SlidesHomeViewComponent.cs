using Microsoft.AspNetCore.Mvc;
using TruongCongHuy_64130895.Models;
using Microsoft.EntityFrameworkCore;
namespace TruongCongHuy_64130895.ViewComponents
{
    public class SlidesHomeViewComponent:  ViewComponent
    {
        private readonly ShopOnlineSalesContext context;

        public SlidesHomeViewComponent(ShopOnlineSalesContext context)
        {
            this.context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int? limit)
        {
            int limitProduct = limit ?? 3;
            var products = await context.Products.Include(p => p.Category).Take(limitProduct).OrderBy(p=>p.Price).ToListAsync();

            return View("SlidesHome", products);

        }
    }
}
