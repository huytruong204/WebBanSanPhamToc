  using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TruongCongHuy_64130895.Models;
using TruongCongHuy_64130895.ViewModels;
using X.PagedList;
namespace TruongCongHuy_64130895.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly ShopOnlineSalesContext context;
        public SanPhamController(ShopOnlineSalesContext context)
        {
            this.context = context;
        }
        public async Task<IActionResult> Index(string? TimKiem = "" ,string? SapXep="Tất cả", int? idcategory = 0, int page = 1, int pagesize = 9)
        {
            var sp = context.Products.Include(c => c.Category).AsQueryable();
            if (idcategory != 0)
                sp = sp.Where(sp => sp.CategoryId == idcategory);

            if(TimKiem != null)
                sp = sp.Where(s=>s.Name.Contains(TimKiem));

            switch (SapXep)
            {
                case "Giá thấp đến cao":
                    sp = sp.OrderBy(sp => sp.IsOnSale ? sp.PriceSales : sp.Price);
                    break;
                case "Giá cao đến thấp":
                    sp = sp.OrderByDescending(sp => sp.IsOnSale ? sp.PriceSales : sp.Price);
                    break;
                case "Tên: A đến Z":
                    sp = sp.OrderBy(sp => sp.Name);
                    break;
                case "Tên: Z đến A":
                    sp = sp.OrderByDescending(sp => sp.Name);
                    break;
                case "Sản phẩm đang sale":
                    sp = sp.OrderBy(sp => sp.PriceSales).Where(t=>t.IsOnSale == true);
                    break;
                default:
                    sp = sp.OrderBy(sp => sp.Id);
                    break;
            }

            var order = new List<string> {
                "Tất cả",
                "Giá thấp đến cao",
                "Giá cao đến thấp",
                "Tên: A đến Z",
                "Tên: Z đến A",
                "Sản phẩm đang sale",
                };
            ViewBag.order = new SelectList(order, SapXep);

            ViewData["SapXep"] = Request.Query["SapXep"].ToString();

            ViewBag.TimKiem = TimKiem;

            var pagelist = await sp.ToPagedListAsync(page, pagesize);
            return View(pagelist);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var sp = await context.Products.Include(c => c.Category).FirstOrDefaultAsync(m=>m.Id == id);
            if (sp == null)
            {
                return NotFound();
            }
            return View(sp);

        }
    }
}
