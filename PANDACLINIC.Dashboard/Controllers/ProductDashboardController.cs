using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PANDACLINIC.Application.DTOS.Product;
using PANDACLINIC.Application.InterfacesService.ProductService;

namespace PANDACLINIC.Dashboard.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class ProductDashboardController : Controller
    {
        private readonly IProductService _productService;

        public ProductDashboardController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _productService.GetAllAsync();
            return View(result.Data ?? Enumerable.Empty<ProductSummaryDto>());
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RecycleBin()
        {
            var result = await _productService.GetDeletedProductsAsync();
            return View(result.Data ?? Enumerable.Empty<ProductSummaryDto>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ProductRequestDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductRequestDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var result = await _productService.CreateAsync(dto);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Product created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault() ?? result.Message);
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            var dto = new ProductRequestDto
            {
                Name = result.Data.Name,
                Description = result.Data.Description,
                Price = result.Data.Price,
                Currency = result.Data.Currency,
                Weight = result.Data.Weight,
                Taste = result.Data.Taste,
                ImageUrl = result.Data.ImageUrl,
                DiscountPercentage = result.Data.DiscountPercentage,
                Stock = result.Data.Stock,
                IsActive = result.Data.IsActive,
                Type = result.Data.Type
            };

            ViewBag.ProductId = id;
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProductRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ProductId = id;
                return View(dto);
            }

            var result = await _productService.UpdateAsync(id, dto);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault() ?? result.Message);
            ViewBag.ProductId = id;
            return View(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _productService.DeleteAsync(id);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess
                ? result.Message
                : result.Errors.FirstOrDefault() ?? result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id)
        {
            var result = await _productService.RestoreAsync(id);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess
                ? result.Message
                : result.Errors.FirstOrDefault() ?? result.Message;
            return RedirectToAction(result.IsSuccess ? nameof(Index) : nameof(RecycleBin));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(Guid id, bool isActive)
        {
            var result = await _productService.ToggleActiveAsync(id, isActive);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess
                ? result.Message
                : result.Errors.FirstOrDefault() ?? result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
