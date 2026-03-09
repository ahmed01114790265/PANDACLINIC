using PANDACLINIC.Shared.Enums;

namespace PANDACLINIC.Application.DTOS.Product
{
    public class ProductDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = "EGY";
        public decimal Weight { get; set; }
        public string Taste { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = "default-product.png";
        public int? DiscountPercentage { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public ProductType Type { get; set; }
    }
}
