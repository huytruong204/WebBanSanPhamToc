using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TruongCongHuy_64130895.Helper;
using TruongCongHuy_64130895.Models;
using TruongCongHuy_64130895.ViewModels;

namespace TruongCongHuy_64130895.Controllers
{
    public class GioHangController : Controller
    {
        private readonly ShopOnlineSalesContext context;
        private readonly IConfiguration configuration;

        public GioHangController(ShopOnlineSalesContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;

        }
        public List<CartItem> cart => HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
        public IActionResult Index()
        {
            ViewBag.Subtotal = cart.Sum(item => item.UnitPrice * item.Quantity);
            return View(cart);
        }

        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            if (quantity <= 0)
            {
                return BadRequest("Số lượng phải lớn hơn 0."); 
            }
            var gioHang = cart;
            var item = gioHang.SingleOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                item.Quantity += quantity;
            }
            else
            {
                var product = await context.Products.FindAsync(productId);

                if (product == null)
                {
                    return NotFound("Sản phẩm không tồn tại.");
                }

                item = new CartItem
                {
                    ProductId = product.Id,
                    Quantity = quantity,
                    Img = product.ImageUrl!,
                    ProductName = product.Name,
                };
                if (product.IsOnSale)
                {
                    item.UnitPrice = product.PriceSales.Value;
                }
                else
                {
                    item.UnitPrice = product.Price;
                }


                gioHang.Add(item);
            }

            HttpContext.Session.SetObject("Cart", gioHang);
            return Json(new { success = true, message = "Đã thêm sản phẩm vào giỏ hàng!" });
        }
        public IActionResult UpdateCart(int productId, int quantity = 1, bool isIncrement = true)
        {
            var gioHang = cart;

            var item = gioHang.SingleOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
              
                if (isIncrement)
                {
                    item.Quantity += quantity;
                }
                else
                {
                    item.Quantity -= quantity;
                    if (item.Quantity <= 0)
                    {
                        gioHang.Remove(item);
                    }
                }
            }
            var updatedCart = gioHang.Select(c => new
            {
                c.ProductId,
                c.ProductName,
                c.Quantity,
                Total = c.Quantity * c.UnitPrice
            }).ToList();

            var subtotal = gioHang.Sum(item => item.UnitPrice * item.Quantity);
            HttpContext.Session.SetObject("Cart", gioHang);

            return Json(new { success = true, updatedCart, subtotal });
        }
        public IActionResult RemoveFromCart(int productId)
        {
            var gioHang = cart;

            var item = gioHang.SingleOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                gioHang.Remove(item);
                
            }
            HttpContext.Session.SetObject("Cart", gioHang);

            var subtotal = gioHang.Sum(item => item.UnitPrice * item.Quantity);

            return Json(new { success = true, subtotal });
        }
        [Authorize]
        public IActionResult Checkout()
        {
            var gioHang = cart;

            if (gioHang == null || !gioHang.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index"); 
            }

            ViewBag.Subtotal = gioHang.Sum(item => item.UnitPrice * item.Quantity);

            return View(gioHang);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(string userId, string paymentMethod)
        {

            var gioHang = cart;
            try
            {
                if (paymentMethod == "VNPAY")
                {
                   
                    var orderTemp = new
                    {
                        UserId = userId,
                        OrderDate = DateTime.Now,
                        TotalAmount = gioHang.Sum(c => c.UnitPrice * c.Quantity),
                        CartItems = gioHang
                    };

                    TempData["OrderTemp"] = System.Text.Json.JsonSerializer.Serialize(orderTemp);

                    var vnPayHelper = new VNPayHelper(configuration);
                    var paymentUrl = vnPayHelper.CreatePaymentUrl(orderTemp.GetHashCode(), orderTemp.TotalAmount, HttpContext);
                    return Redirect(paymentUrl);
                }
                else
                {
                    var order = new Order
                    {
                        UserId = userId,
                        OrderDate = DateTime.Now,
                        TotalAmount = gioHang.Sum(c => c.UnitPrice * c.Quantity),
                        Status = "Chờ xác nhận"
                    };
                    context.Orders.Add(order);
                    await context.SaveChangesAsync();

                    foreach (var item in gioHang)
                    {
                        var orderDetail = new OrderDetail
                        {
                            OrderId = order.Id,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice
                        };
                        context.OrderDetails.Add(orderDetail);
                    }

                    var payment = new Payment
                    {
                        OrderId = order.Id,
                        PaymentDate = DateTime.Now,
                        PaymentMethod = paymentMethod,
                        Amount = order.TotalAmount
                    };
                    context.Payments.Add(payment);
                    await context.SaveChangesAsync();

                    HttpContext.Session.Remove("Cart");

                    TempData["Message"] = "Thanh toán thành công! Đơn hàng của bạn đang được xử lý.";
                    return RedirectToAction("Index", "TheoDoiDH");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
        public IActionResult PaymentReturn()
        {
            var vnPayHelper = new VNPayHelper(configuration);
            var response = vnPayHelper.ProcessReturnRequest(HttpContext.Request.Query);

            if (response.Success)
            {
                var orderTempJson = TempData["OrderTemp"] as string;
                if (!string.IsNullOrEmpty(orderTempJson))
                {
                    var orderTemp = System.Text.Json.JsonSerializer.Deserialize<OrderTemp>(orderTempJson);

                    var order = new Order
                    {
                        UserId = orderTemp.UserId,
                        OrderDate = orderTemp.OrderDate,
                        TotalAmount = orderTemp.TotalAmount,
                        Status = "Đang xử lý"
                    };
                    context.Orders.Add(order);
                    context.SaveChanges();

                    foreach (var item in orderTemp.CartItems)
                    {
                        var orderDetail = new OrderDetail
                        {
                            OrderId = order.Id,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice
                        };
                        context.OrderDetails.Add(orderDetail);
                    }

                    var payment = new Payment
                    {
                        OrderId = order.Id,
                        PaymentDate = DateTime.Now,
                        PaymentMethod = "VNPAY",
                        Amount = order.TotalAmount
                    };
                    context.Payments.Add(payment);

                    context.SaveChanges();

                    HttpContext.Session.Remove("Cart");
                }
            }

            return RedirectToAction("Index", "TheoDoiDH");
        }
        public IActionResult RefreshCartViewComponent()
        {
            return ViewComponent("Cart");
        }
    }

}
