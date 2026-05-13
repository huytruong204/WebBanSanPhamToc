using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TruongCongHuy_64130895.ViewModels;
using TruongCongHuy_64130895.Models;
using Microsoft.EntityFrameworkCore;
using TruongCongHuy_64130895.Helper;
using Microsoft.AspNetCore.Authorization;


namespace TruongCongHuy_64130895.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<Customers> signInManager;
        private readonly UserManager<Customers> userManager;
        public KhachHangController(SignInManager<Customers> signInManager, UserManager<Customers> userManager, IConfiguration _configuration)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this._configuration = _configuration;

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Customer()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name!);
                if (user != null)
                {
                    return View(user); 
                }
            }
            return RedirectToAction("Login", "KhachHang");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var customer = await userManager.FindByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            var model = new EditCustomerViewModel
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                Avatar = customer.avatar
            };

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCustomerViewModel model, IFormFile? Avatar)
        {
            if (ModelState.IsValid)
            {


                var customer = await userManager.FindByIdAsync(model.Id);
                if (customer == null)
                {
                    return NotFound();
                }

                customer.FullName = model.FullName;

                var existing = await userManager.FindByEmailAsync(customer.Email);
                if (existing != null && existing.Email != customer.Email)
                {
                    TempData["Error"] = "Email này đã tồn tại trong hệ thống";
                    return View(model);
                }
                customer.Email = model.Email;
                customer.PhoneNumber = model.PhoneNumber;
                customer.Address = model.Address;
                if (!string.IsNullOrEmpty(model.OldPassword) && !string.IsNullOrEmpty(model.NewPassword))
                {
                    var passwordChangeResult = await userManager.ChangePasswordAsync(customer, model.OldPassword, model.NewPassword);
                    if (!passwordChangeResult.Succeeded)
                    {
                        TempData["Error"] = "Mật khẩu cũ không đúng";
                        return View(model);
                    }
                }
                if (Avatar != null)
                {
                    var imageName = ImageHelper.UploadImage(Avatar, "Image/Avatar");
                    model.Avatar = imageName;
                }
                else
                {
                    model.Avatar = customer.avatar; 
                }
                customer.avatar = model.Avatar;

                var result = await userManager.UpdateAsync(customer);
                if (result.Succeeded)
                {
                    return RedirectToAction("Customer", "KhachHang", new { id = customer.Id });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }


        public IActionResult Login()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM lg)
        {
            if(ModelState.IsValid)
            {
                Customers? user = await userManager.FindByEmailAsync(lg.EmailOrUsername!)
                       ?? await userManager.FindByNameAsync(lg.EmailOrUsername!);
                if (user == null)
                {
                    ModelState.AddModelError("", "Đăng nhập không hợp lệ.");
                    return View(lg);
                }
                if (!await userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError("", "Email chưa được xác nhận. Vui lòng xác nhận email của bạn trước.");
                    return View(lg);
                }

                var result = await signInManager.PasswordSignInAsync(user.UserName!, lg.Password!, lg.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Customer", "KhachHang");
                }

                ModelState.AddModelError("", "Đăng nhập không hợp lệ.");
            }
            return View(lg);
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM rg)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await userManager.FindByEmailAsync(rg.Email) ?? await userManager.FindByNameAsync(rg.UserName);
                if (existingUser != null)
                {
                    TempData["Error"] = "Tài khoản hoặc Email này đã tồn tại trong hệ thống";
                    return View(rg);
                }

                Customers user = new Customers()
                {
                    FullName = rg.FullName,
                    Email = rg.Email,
                    UserName = rg.UserName,
                    Address = rg.Address,
                };

                var result = await userManager.CreateAsync(user, rg.Password!);

                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Email", new { token, email = user.Email }, Request.Scheme);

                    EmailHelper emailHelper = new EmailHelper(_configuration, "Activation");
                    var emailResponse = emailHelper.SendEmail(user.Email!, user.FullName!, confirmationLink!);

                    if (emailResponse.isSuccess)
                    {
                        await userManager.AddToRoleAsync(user, "Khách Hàng");
                        await signInManager.SignInAsync(user, false);
                        return RedirectToAction("Login", "KhachHang");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Lỗi gửi mail: " + emailResponse.errorMessage);
                    }
                }
                else 
                {
                    foreach (var error in result.Errors)
                    {
                        if (error.Code.Contains("Password"))
                        {
                            ModelState.AddModelError("Password", error.Description);
                        }
                        else if (error.Code.Contains("UserName"))
                        {
                            ModelState.AddModelError("UserName", error.Description);
                        }
                        else if (error.Code.Contains("Email"))
                        {
                            ModelState.AddModelError("Email", error.Description);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }

            return View(rg);
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

		public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM fg)
        {
            if(ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(fg.Email!);
                if (user == null)
                {
                    ModelState.AddModelError("", "Nếu email tồn tại trong hệ thống, bạn sẽ nhận được hướng dẫn qua email.");
                    return View(fg);
                }
                var token = await userManager.GeneratePasswordResetTokenAsync(user!);

                var resetLink = Url.Action("ResetPassword", "KhachHang", new { userId = user.Id, token = token }, Request.Scheme);

                EmailHelper emailHelper = new EmailHelper(_configuration, "ForgotPassword");
                var emailResponse = emailHelper.SendEmail(user.Email!, user.FullName!, resetLink!);
                if (!emailResponse.isSuccess)
                {
                    ModelState.AddModelError("", "Không thể gửi email. Vui lòng thử lại sau.");
                    return View(fg);
                }
                else
                {
                    TempData["Success"] = "Đã gửi mail thành công";
                }
            }
            return View(fg);
        }
        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest("Thông tin không hợp lệ.");
            }
            var model = new ResetPasswordVM
            {
                UserId = userId,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM rs)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(rs.UserId!);
                if (user == null)
                {
                    ModelState.AddModelError("", "Người dùng không tồn tại.");
                    return View(rs);
                }
                var result = await userManager.ResetPasswordAsync(user!, rs.Token!, rs.NewPassword!);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "KhachHang");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            return View(rs);
        }
    }
}
