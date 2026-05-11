using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using TruongCongHuy_64130895.Models;
using X.PagedList;

namespace TruongCongHuy_64130895.ViewComponents
{
    public class DanhGiaViewComponent : ViewComponent
    {
        private readonly ShopOnlineSalesContext _context;

        public DanhGiaViewComponent(ShopOnlineSalesContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int productId, int page=1, int pageSize=5)
        {
            var reviews = await _context.Reviews.Where(r => r.ProductId == productId).OrderByDescending(r => r.ReviewId).Include(t=>t.Product).ToListAsync();
            if (!reviews.Any())
            {
                TempData["RV"] = "Chưa có đánh giá cho sản phẩm này!";
            }
            var pagedList = reviews.ToPagedList(page, pageSize);

            return View("DanhGia", pagedList);
        }
    }

}
