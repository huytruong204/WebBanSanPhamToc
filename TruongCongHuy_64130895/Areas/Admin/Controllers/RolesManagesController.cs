using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TruongCongHuy_64130895.Models;

namespace ruongCongHuy_64130895.Controllers
{
    [Area("Admin")]

    [Authorize(Roles = "Quản Trị Viên")]
    public class RolesManagesController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManage;
        public RolesManagesController(RoleManager<IdentityRole> rolesManage)
        {
            this.roleManage = rolesManage;
        }

        // GET: RolesManages
        public async Task<IActionResult> Index()
        {
            return View(await roleManage.Roles.ToListAsync());
        }
        public IActionResult create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var role = new IdentityRole(roleName);
                var result = await roleManage.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View();
        }
        public async Task<IActionResult> Edit(string id)
        {
            var role = await roleManage.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                var existingRole = await roleManage.FindByIdAsync(id);
                if (existingRole == null)
                {
                    return NotFound();
                }

                // Update the role's name
                existingRole.Name = role.Name;

                var result = await roleManage.UpdateAsync(existingRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(role);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var role = await roleManage.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var role = await roleManage.FindByIdAsync(id);
            if (role != null)
            {
                var result = await roleManage.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}
