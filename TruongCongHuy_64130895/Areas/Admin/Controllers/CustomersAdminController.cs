using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TruongCongHuy_64130895.Models;
using X.PagedList;
namespace TruongCongHuy_64130895.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Quản Trị Viên")]
    public class CustomersAdminController : Controller
    {
        private readonly UserManager<Customers> userManager;
        public CustomersAdminController(UserManager<Customers> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index(string? TimKiem="",int page = 1, int pagesize=5)
        {
            var user = userManager.Users.ToList().AsQueryable();
            if (TimKiem != null)
                user = user.Where(t => t.FullName.Contains(TimKiem));

            return View(await user.ToPagedListAsync(page, pagesize));
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string userID)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(t => t.Id == userID);
            if (user == null)
            {
                return NotFound($"Không tìm thấy khách hàng với ID {userID}.");
            }
            try
            {
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Xóa khách hàng thành công.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể xóa khách hàng.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi khi xóa khách hàng: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
