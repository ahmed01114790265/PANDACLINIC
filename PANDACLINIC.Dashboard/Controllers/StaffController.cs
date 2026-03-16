using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PANDACLINIC.Dashboard.Models.AccountViewModel;
using PANDACLINIC.Domain.Entity;

namespace PANDACLINIC.Dashboard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StaffController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public StaffController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var staffUsers = await _userManager.GetUsersInRoleAsync("Staff");
            var active = staffUsers.Where(u => !u.LockoutEnd.HasValue || u.LockoutEnd <= DateTimeOffset.UtcNow).ToList();
            return View(active);
        }

        [HttpGet]
        public IActionResult Create() => View(new CreateStaffVM());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStaffVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                fullName = model.FullName,
                PhoneNumber = model.PhoneNumber
            };

            if (!await _roleManager.RoleExistsAsync("Staff"))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>("Staff"));
            }

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Staff");
                TempData["SuccessMessage"] = "Staff account created successfully.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    
        [HttpGet]
        public async Task<IActionResult> RecycleBin()
        {
            var staffUsers = await _userManager.GetUsersInRoleAsync("Staff");
            var deleted = staffUsers.Where(u => u.LockoutEnd.HasValue && u.LockoutEnd > DateTimeOffset.UtcNow).ToList();
            return View(deleted);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null || !await _userManager.IsInRoleAsync(user, "Staff"))
            {
                TempData["ErrorMessage"] = "Staff not found.";
                return RedirectToAction(nameof(Index));
            }

            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;
            await _userManager.UpdateAsync(user);
            TempData["SuccessMessage"] = "Staff removed.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                TempData["ErrorMessage"] = "Staff not found.";
                return RedirectToAction(nameof(RecycleBin));
            }

            user.LockoutEnd = null;
            user.LockoutEnabled = false;
            if (!await _userManager.IsInRoleAsync(user, "Staff"))
            {
                await _userManager.AddToRoleAsync(user, "Staff");
            }

            await _userManager.UpdateAsync(user);
            TempData["SuccessMessage"] = "Staff restored.";
            return RedirectToAction(nameof(Index));
        }
    }
}
