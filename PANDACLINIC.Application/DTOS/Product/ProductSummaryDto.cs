using PANDACLINIC.Shared.Enums;

namespace PANDACLINIC.Application.DTOS.Product
{
    public class ProductSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = "EGY";
        public string ImageUrl { get; set; } = "default-product.png";
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public int? DiscountPercentage { get; set; }
        public ProductType Type { get; set; }
    }
}
