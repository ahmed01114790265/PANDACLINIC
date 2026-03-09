using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Web.Models.AccountViewModel;

namespace PANDACLINIC.Web.Controllers.AccountController
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register() => View();

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterPhoneVM model)
        {
            if (ModelState.IsValid)
            {
                var normalizedPhone = model.PhoneNumber?.Trim();
                var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == normalizedPhone);
                if (existingUser != null)
                {
                    ModelState.AddModelError(nameof(model.PhoneNumber), "رقم الهاتف مستخدم بالفعل.");
                    return View(model);
                }

                var user = new ApplicationUser
                {
                    UserName = normalizedPhone,
                    PhoneNumber = normalizedPhone,
                    fullName = model.FullName.Trim()
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Product");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.PendingAddToCart = !string.IsNullOrWhiteSpace(returnUrl)
                && returnUrl.Contains("/Order/AddToCart", StringComparison.OrdinalIgnoreCase);
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginPhoneVM model, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.PendingAddToCart = !string.IsNullOrWhiteSpace(returnUrl)
                && returnUrl.Contains("/Order/AddToCart", StringComparison.OrdinalIgnoreCase);

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.PhoneNumber.Trim(), model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "Product");
                }

                ModelState.AddModelError(string.Empty, "رقم الهاتف أو كلمة المرور غير صحيحة.");
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Product");
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword() => View();

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "تم تغيير كلمة المرور بنجاح.";
                return RedirectToAction("Index", "Product");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }
    }
}
