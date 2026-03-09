using Microsoft.AspNetCore.Http;
using PANDACLINIC.Shared.Enums;

namespace PANDACLINIC.Application.DTOS.Product
{
    public class ProductRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = "EGY";
        public decimal Weight { get; set; }
        public string Taste { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = "default-product.png";
        public IFormFile? ImageFile { get; set; }
        public int? DiscountPercentage { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; } = true;
        public ProductType Type { get; set; }
    }
}
