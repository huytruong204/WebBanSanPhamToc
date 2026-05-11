using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using TruongCongHuy_64130895.Areas.Admin.Models;
using TruongCongHuy_64130895.Models;


namespace TruongCongHuy_64130895.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Quản Trị Viên")]
    public class ManageUserRolesController : Controller
    {
        private readonly UserManager<Customers> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public ManageUserRolesController(UserManager<Customers> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = userManager.Users.ToList();
            var userRolesViewModel = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                userRolesViewModel.Add(new UserRolesViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Roles = roles
                });
            }

            return View(userRolesViewModel);
        }
        public async Task<IActionResult> AddRoleToUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var roles = roleManager.Roles.ToList();
            var model = new AddRoleToUserViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = roles.Select(r => r.Name).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRoleToUser(AddRoleToUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var userRoles = await userManager.GetRolesAsync(user);
            if (userRoles.Contains(model.SelectedRole))
            {
                TempData["message"] = "Người dùng đã có vai trò này";
                return View(model);
            }

            if (!await roleManager.RoleExistsAsync(model.SelectedRole))
            {
                ModelState.AddModelError(string.Empty, "Vai trò không tồn tại.");
                return View(model);
            }


            var result = await userManager.AddToRoleAsync(user, model.SelectedRole);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRoleFromUser(string userId, string roleName)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (!await roleManager.RoleExistsAsync(roleName))
            {
                ModelState.AddModelError(string.Empty, "Vai trò không tồn tại.");
                return RedirectToAction("Index");
            }

            var result = await userManager.RemoveFromRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }



    }
}
