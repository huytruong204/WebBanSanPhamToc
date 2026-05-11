using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using TruongCongHuy_64130895.Helper;
using TruongCongHuy_64130895.ViewModels;
namespace TruongCongHuy_64130895.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
            return View(new CartModel()
            {
                Quantity = cart.Sum(p => p.Quantity),
                Total = (int)cart.Sum(p => p.Total)
            });
        }
    }
}
