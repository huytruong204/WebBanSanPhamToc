using TruongCongHuy_64130895.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TruongCongHuy_64130895.ViewComponents
{
    public class DanhMucMenuViewComponent : ViewComponent
    {
        private readonly ShopOnlineSalesContext context;
        public DanhMucMenuViewComponent(ShopOnlineSalesContext context)
        {

            this.context = context;

        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var DMmenu = await context.Categories
           .Select(category => new
           {
               Category = category,
               ProductCount = context.Products.Count(p => p.CategoryId == category.Id)
           })
           .ToListAsync();

            return View("Default", DMmenu);
        }
    }
}
