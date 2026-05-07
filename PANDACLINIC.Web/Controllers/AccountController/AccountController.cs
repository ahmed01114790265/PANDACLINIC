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
        private static readonly HashSet<string> AllowedAccountTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "Admin",
            "Staff",
            "Customer"
        };

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
                    return await RedirectAfterLoginAsync(user, returnUrl: null);
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

            if (!AllowedAccountTypes.Contains(model.AccountType))
            {
                ModelState.AddModelError(nameof(model.AccountType), "نوع الحساب غير صحيح.");
            }

            if (ModelState.IsValid)
            {
                var user = await FindLoginUserAsync(model);

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password, model.RememberMe, false);

                    if (result.Succeeded)
                    {
                        // If the user entered an email, we treat this as an admin/staff login attempt and redirect
                        // based on the user's real roles. This avoids "can't login" when the UI is left on Customer.
                        if (string.IsNullOrWhiteSpace(model.Email) && !await _userManager.IsInRoleAsync(user, model.AccountType))
                        {
                            await _signInManager.SignOutAsync();
                            ModelState.AddModelError(nameof(model.AccountType), "نوع الحساب المختار لا يطابق هذا المستخدم.");
                            return View(model);
                        }

                        return await RedirectAfterLoginAsync(user, returnUrl);
                    }
                }

                ModelState.AddModelError(string.Empty, "بيانات الدخول أو كلمة المرور غير صحيحة.");
            }

            return View(model);
        }

        private async Task<ApplicationUser?> FindLoginUserAsync(LoginPhoneVM model)
        {
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                return await _userManager.FindByEmailAsync(model.Email.Trim());
            }

            if (model.AccountType == "Customer")
            {
                var phoneNumber = model.PhoneNumber?.Trim();
                return await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            }

            return await _userManager.FindByEmailAsync(model.Email!.Trim());
        }

        private async Task<IActionResult> RedirectAfterLoginAsync(ApplicationUser user, string? returnUrl)
        {
            if (await _userManager.IsInRoleAsync(user, "Admin") || await _userManager.IsInRoleAsync(user, "Staff"))
            {
                return RedirectToAction("Index", "Home", new { area = "Dashboard" });
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("MyAnimals", "Animal");
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
