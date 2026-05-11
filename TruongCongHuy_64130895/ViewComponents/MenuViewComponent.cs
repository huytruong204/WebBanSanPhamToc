using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using TruongCongHuy_64130895.Models;

namespace TruongCongHuy_64130895.ViewComponents
{
    public class MenuViewComponent: ViewComponent
    {
        private readonly ShopOnlineSalesContext context;

        public MenuViewComponent(ShopOnlineSalesContext context)
        {
            this.context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //Lấy danh sách menu từ database
            var menus = await context.Menus
                .Where(m => m.IsVisible)
                .OrderBy(m => m.MenuIndex)
                .ToListAsync();

			if (!User.IsInRole("Quản Trị Viên") && !User.IsInRole("Nhân Viên"))
			{
				menus = menus.Where(m => m.Title != "Trang quản trị").ToList();
			}

			// Trả về view cùng với dữ liệu
			return View("Menu", menus);
        }
    }
}
