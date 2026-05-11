using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TruongCongHuy_64130895.Models;

namespace TruongCongHuy_64130895.Controllers
{
    public class EmailController : Controller
    {
        private readonly UserManager<Customers> userManager;
        public EmailController(UserManager<Customers> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return RedirectToAction(nameof(Error));
            }
            var result = await userManager.ConfirmEmailAsync(user, token);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }
        public IActionResult Error()
        {
            return View();
        }
    }
}
