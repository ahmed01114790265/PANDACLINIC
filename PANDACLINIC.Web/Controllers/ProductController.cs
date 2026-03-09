using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PANDACLINIC.Application.InterfacesService.ProductService;
using PANDACLINIC.Shared.Enums;

namespace PANDACLINIC.Web.Controllers
{
    [AllowAnonymous]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search = null, ProductType? type = null)
        {
            var result = await _productService.GetStoreProductsAsync(search, type);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
                return View(Enumerable.Empty<PANDACLINIC.Application.DTOS.Product.ProductSummaryDto>());
            }

            ViewBag.Search = search;
            ViewBag.Type = type;
            return View(result.Data ?? Enumerable.Empty<PANDACLINIC.Application.DTOS.Product.ProductSummaryDto>());
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (!result.IsSuccess || result.Data == null)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }
    }
}


