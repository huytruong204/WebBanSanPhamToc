using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TruongCongHuy_64130895.Models;
using TruongCongHuy_64130895.ViewModels;
namespace TruongCongHuy_64130895.ViewComponents
{
    public class SanPhamNoiBatViewComponent : ViewComponent
    {
        private readonly ShopOnlineSalesContext context;
        public SanPhamNoiBatViewComponent(ShopOnlineSalesContext context)
        {
            this.context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int? limit)
        {
            int limitProduct = limit ?? 8;
            var products = await context.Products.Include(p => p.Category).Take(limitProduct).ToListAsync();
            var reviews = await context.Reviews.Where(r => products.Select(p => p.Id).Contains(r.ProductId)).ToListAsync();

            var spvm = products.Select(product =>
            {
                var productReviews = reviews.Where(r => r.ProductId == product.Id);
                var avgRating = productReviews.Any() ? productReviews.Average(r => r.Rating) : 0;

                return new ProductVM
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    PriceSales = product.PriceSales.GetValueOrDefault(),
                    IsOnSale = product.IsOnSale,
                    Img = product.ImageUrl,
                    Ratings = (int)Math.Round(avgRating) 
                };
            }).OrderByDescending(r=>r.Ratings).ToList();
            return View("SanPhamNoiBat", spvm);

        }
    }
}
