using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PANDACLINIC.Application.DTOS.Order;
using PANDACLINIC.Application.DTOS.Product;
using PANDACLINIC.Application.InterfacesService.OrderService;
using PANDACLINIC.Application.InterfacesService.ProductService;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Web.Models.Order;

namespace PANDACLINIC.Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private const string CartSessionKey = "PandaClinicCart";

        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public OrderController(
            IOrderService orderService,
            IProductService productService,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _orderService = orderService;
            _productService = productService;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            var result = await _orderService.GetByCustomerAsync(userId.Value);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
                return View(Enumerable.Empty<OrderSummaryDto>());
            }

            return View((result.Data ?? Enumerable.Empty<OrderSummaryDto>()).OrderByDescending(o => o.CreatedAt));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            var result = await _orderService.GetDetailsAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(MyOrders));
            }

            if (result.Data.UserId != userId.Value)
                return Forbid();

            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            var viewItems = await BuildCartItemsAsync();
            ViewBag.Total = viewItems.Sum(i => i.LineTotal);
            return View(viewItems);
        }

        [HttpGet]
        public async Task<IActionResult> AddToCart(Guid productId, int quantity = 1)
        {
            return await AddToCartCore(productId, quantity);
        }

        [HttpPost]
        [ActionName("AddToCart")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCartPost(Guid productId, int quantity = 1)
        {
            return await AddToCartCore(productId, quantity);
        }

        private async Task<IActionResult> AddToCartCore(Guid productId, int quantity)
        {
            if (quantity <= 0) quantity = 1;

            var productResult = await _productService.GetByIdAsync(productId);
            if (!productResult.IsSuccess || productResult.Data == null || !productResult.Data.IsActive)
            {
                TempData["ErrorMessage"] = "المنتج غير متاح حالياً.";
                return RedirectToAction("Index", "Product");
            }

            var cart = GetCart();
            var existing = cart.FirstOrDefault(c => c.ProductId == productId);
            if (existing == null)
                cart.Add(new CartSessionItem { ProductId = productId, Quantity = quantity });
            else
                existing.Quantity += quantity;

            SaveCart(cart);
            TempData["SuccessMessage"] = "تمت إضافة المنتج إلى السلة.";
            return RedirectToAction(nameof(Cart));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(Guid productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                if (quantity <= 0)
                    cart.Remove(item);
                else
                    item.Quantity = quantity;

                SaveCart(cart);
            }

            return RedirectToAction(nameof(Cart));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(Guid productId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }

            return RedirectToAction(nameof(Cart));
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            var cartItems = await BuildCartItemsAsync();
            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "السلة فارغة.";
                return RedirectToAction(nameof(Cart));
            }

            var user = await _userManager.FindByIdAsync(userId.ToString()!);
            var model = new OrderCheckoutRequestDto
            {
                ClientName = user?.fullName ?? string.Empty,
                ClientPhone = user?.PhoneNumber ?? string.Empty
            };

            ViewBag.CartItems = cartItems;
            ViewBag.Total = cartItems.Sum(i => i.LineTotal);
            ViewBag.VodafoneCashNumber = _configuration["ManualPayment:VodafoneCashNumber"] ?? "01004293837";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(OrderCheckoutRequestDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            var cartItems = await BuildCartItemsAsync();
            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "السلة فارغة.";
                return RedirectToAction(nameof(Cart));
            }

            dto.Items = cartItems.Select(c => new OrderItemRequestDto
            {
                ProductId = c.ProductId,
                Quantity = c.Quantity
            }).ToList();

            if (string.IsNullOrWhiteSpace(dto.ClientName))
                ModelState.AddModelError(nameof(dto.ClientName), "اسم العميل مطلوب.");

            if (string.IsNullOrWhiteSpace(dto.ClientPhone))
                ModelState.AddModelError(nameof(dto.ClientPhone), "رقم الهاتف مطلوب.");

            if (string.IsNullOrWhiteSpace(dto.ClientAddress))
                ModelState.AddModelError(nameof(dto.ClientAddress), "العنوان مطلوب.");

            if (dto.PaymentMethod != "VodafoneCash" && dto.PaymentMethod != "CashOnDelivery")
                ModelState.AddModelError(nameof(dto.PaymentMethod), "اختر طريقة دفع صحيحة.");

            if (!ModelState.IsValid)
            {
                ViewBag.CartItems = cartItems;
                ViewBag.Total = cartItems.Sum(i => i.LineTotal);
                ViewBag.VodafoneCashNumber = _configuration["ManualPayment:VodafoneCashNumber"] ?? "01004293837";
                return View(dto);
            }

            var result = await _orderService.CreateAsync(userId.Value, dto);
            if (!result.IsSuccess || result.Data == null)
            {
                ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault() ?? result.Message);
                ViewBag.CartItems = cartItems;
                ViewBag.Total = cartItems.Sum(i => i.LineTotal);
                ViewBag.VodafoneCashNumber = _configuration["ManualPayment:VodafoneCashNumber"] ?? "01004293837";
                return View(dto);
            }

            var whatsappUrl = BuildWhatsAppUrl(result.Data, dto.PaymentMethod);
            SaveCart(new List<CartSessionItem>());

            TempData["SuccessMessage"] = "تم تأكيد الطلب. سيتم تحويلك إلى واتساب.";
            return Redirect(whatsappUrl);
        }

        private string BuildWhatsAppUrl(OrderDetailDto order, string paymentMethod)
        {
            
            var rawNumber = (_configuration["ManualPayment:ClinicWhatsAppNumber"] ?? "01004293837")
                            .Replace("+", string.Empty)
                            .TrimStart('0');

            var whatsappNumber = rawNumber.StartsWith("20") ? rawNumber : "20" + rawNumber;
            var vodafoneNumber = _configuration["ManualPayment:VodafoneCashNumber"] ?? "01004293837";

            string paymentText = paymentMethod == "VodafoneCash"
                ? $"سأقوم بالدفع عبر فودافون كاش على الرقم: {vodafoneNumber}."
                : "سأقوم بالدفع عند الاستلام.";

            var message =
                $"مرحباً PandaClinic، تم تأكيد الطلب رقم {order.Id}. الاسم: {order.ClientName}، الهاتف: {order.ClientPhone}، العنوان: {order.ClientAddress}. {paymentText} الإجمالي: {order.TotalAmount:0.##} جنيه.";

            var encoded = Uri.EscapeDataString(message);
            return $"https://wa.me/{whatsappNumber}?text={encoded}";
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdClaim)) return null;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        private List<CartSessionItem> GetCart()
        {
            var json = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrWhiteSpace(json)) return new List<CartSessionItem>();

            try
            {
                return JsonSerializer.Deserialize<List<CartSessionItem>>(json) ?? new List<CartSessionItem>();
            }
            catch
            {
                return new List<CartSessionItem>();
            }
        }

        private void SaveCart(List<CartSessionItem> cart)
        {
            HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
        }

        private async Task<List<OrderItemSummaryDto>> BuildCartItemsAsync()
        {
            var cart = GetCart();
            if (!cart.Any()) return new List<OrderItemSummaryDto>();

            var productsResult = await _productService.GetAllAsync();
            var products = productsResult.IsSuccess
                ? (productsResult.Data ?? Enumerable.Empty<ProductSummaryDto>()).ToDictionary(p => p.Id, p => p)
                : new Dictionary<Guid, ProductSummaryDto>();

            var items = new List<OrderItemSummaryDto>();
            foreach (var item in cart)
            {
                if (!products.TryGetValue(item.ProductId, out var product) || !product.IsActive) continue;

                var unitPrice = product.Price;
                if (product.DiscountPercentage.HasValue && product.DiscountPercentage.Value > 0)
                    unitPrice -= (unitPrice * product.DiscountPercentage.Value / 100m);

                items.Add(new OrderItemSummaryDto
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    LineTotal = unitPrice * item.Quantity
                });
            }

            return items;
        }
    }
}
