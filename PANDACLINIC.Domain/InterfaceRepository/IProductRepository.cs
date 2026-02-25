using PANDACLINIC.Domain.Comman.GenericRepository;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Domain.InterfaceRepository
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetInStockProductsAsync();

        // Filtering by type (e.g., all "Food" or all "Medicine")
        Task<IEnumerable<Product>> GetByTypeAsync(ProductType type);

        // For "Sale" or "Offers" page
        Task<IEnumerable<Product>> GetDiscountedProductsAsync();

        // Filtering by Taste (useful for pet food preferences)
        Task<IEnumerable<Product>> GetByTasteAsync(string taste);

        // --- Search 
        // Search across Name and Description
        Task<IEnumerable<Product>> SearchAsync(string searchTerm);

        // Sorting by Price or Weight (Dashboard or Storefront)
        Task<IEnumerable<Product>> GetPagedProductsAsync(int pageNumber, int pageSize, ProductType? type = null, string? searchTerm = null);

        // --- Inventory & Admin Management ---
        // Products that are active but have 0 stock
        Task<IEnumerable<Product>> GetOutOfStockProductsAsync();

        // Alert system: Products below a certain stock level
        Task<IEnumerable<Product>> GetLowStockAlertsAsync(int threshold);

        // Toggle status without deleting
        Task UpdateProductStatusAsync(Guid productId, bool isActive);

        // --- Analytics & Statistics ---
        // Most popular products based on OrderItems
        Task<IEnumerable<Product>> GetBestSellersAsync(int topCount);

        // Total value of current inventory (Sum of Price * Stock)
        Task<decimal> GetTotalInventoryValueAsync();

        // Check if a name is unique before creating
        Task<bool> IsProductNameUniqueAsync(string name);
    }
}
