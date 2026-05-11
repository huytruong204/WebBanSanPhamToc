using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TruongCongHuy_64130895.Helper;
using TruongCongHuy_64130895.Models;
using X.PagedList;

namespace TruongCongHuy_64130895.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsAdminController : Controller
    {
        private readonly ShopOnlineSalesContext _context;

        public ProductsAdminController(ShopOnlineSalesContext context)
        {
            _context = context;
        }

        // GET: Admin/ProductsAdmin
        [Authorize(Roles = "Quản Trị Viên, Nhân Viên")]
        public async Task<IActionResult> Index(int DanhMuc = 0, string? TimKiem="", string? SapXep="Name", string? ThuTu = "asc", int page = 1, int pagesize = 5)
        {
            var shopOnlineSalesContext = _context.Products.Include(p => p.Category).AsQueryable();
            if (TimKiem != null)
                shopOnlineSalesContext = shopOnlineSalesContext.Where(p=>p.Name.Contains(TimKiem));
            if (DanhMuc != 0)
                shopOnlineSalesContext = shopOnlineSalesContext.Where(p => p.CategoryId == DanhMuc);

            switch (SapXep)
            {
                case "Name":
                    shopOnlineSalesContext = ThuTu == "asc"
                        ? shopOnlineSalesContext.OrderBy(p => p.Name)
                        : shopOnlineSalesContext.OrderByDescending(p => p.Name);
                    break;
                case "Price":
                    shopOnlineSalesContext = ThuTu == "asc"
                        ? shopOnlineSalesContext.OrderBy(p => p.Price)
                        : shopOnlineSalesContext.OrderByDescending(p => p.Price);
                    break;
                case "PriceSale":
                    shopOnlineSalesContext = ThuTu == "asc"
                        ? shopOnlineSalesContext.OrderBy(p => p.PriceSales)
                        : shopOnlineSalesContext.OrderByDescending(p => p.PriceSales);
                    break;
                case "DiscountPercent":
                    shopOnlineSalesContext = ThuTu == "asc"
                        ? shopOnlineSalesContext.OrderBy(p => p.DiscountPercent)
                        : shopOnlineSalesContext.OrderByDescending(p => p.DiscountPercent);
                    break;
                default:
                    shopOnlineSalesContext = shopOnlineSalesContext.OrderBy(p => p.Id); 
                    break;
            }
            ViewData["SapXep"] = SapXep;
            ViewData["ThuTu"] = ThuTu;
            ViewBag.TimKiem = TimKiem;
            ViewBag.DanhMuc = DanhMuc;
            ViewBag.DanhMucList = new SelectList(_context.Categories, "Id", "Name", DanhMuc);
            return View(await shopOnlineSalesContext.ToPagedListAsync(page, pagesize));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var sp = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);

            return View(sp);
        }

        // GET: Admin/ProductsAdmin/Create
        [Authorize(Roles = "Quản Trị Viên")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Admin/ProductsAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? ImgUpload)
        {
            if (ModelState.IsValid)
            {
                product.CreatedDate = DateTime.Now;
                if(product.IsOnSale && product.DiscountPercent > 0)
                {
                    product.PriceSales = product.Price - ((product.DiscountPercent/100) * product.Price);
                }
                else
                {
                    product.PriceSales = 0;
                }
                if (ImgUpload == null)
                {
                    product.ImageUrl = "";
                }
                else
                {
                    var Name = ImageHelper.UploadImage(ImgUpload, "Admin-template/Images/SanPham"); 
                    product.ImageUrl = Name;
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Admin/ProductsAdmin/Edit/5
        [Authorize(Roles = "Quản Trị Viên")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Admin/ProductsAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? ImgUpload)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (product.IsOnSale && product.DiscountPercent > 0)
                    {
                        product.PriceSales = product.Price - ((product.DiscountPercent / 100) * product.Price);
                    }
                    else
                    {
                        product.PriceSales = 0;
                    }
                    if (ImgUpload != null)
                    {
                        var ImageName = ImageHelper.UploadImage(ImgUpload, "Admin-template/Images/SanPham");
                        product.ImageUrl = ImageName;
                    }
                    else
                    {
                        var existingsp = await _context.Products.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
                        if (existingsp != null)
                        {
                            product.ImageUrl = existingsp.ImageUrl;
                        }
                    }
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Admin/ProductsAdmin/Delete/5
        [Authorize(Roles = "Quản Trị Viên")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/ProductsAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
