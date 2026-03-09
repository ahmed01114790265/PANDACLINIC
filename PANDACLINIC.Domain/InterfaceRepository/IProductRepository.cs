using PANDACLINIC.Domain.Comman.GenericRepository;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Shared.Enums;

namespace PANDACLINIC.Domain.InterfaceRepository
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetInStockProductsAsync();
        Task<IEnumerable<Product>> GetByTypeAsync(ProductType type);
        Task<IEnumerable<Product>> GetDiscountedProductsAsync();
        Task<IEnumerable<Product>> GetByTasteAsync(string taste);
        Task<IEnumerable<Product>> SearchAsync(string searchTerm);
        Task<IEnumerable<Product>> GetPagedProductsAsync(int pageNumber, int pageSize, ProductType? type = null, string? searchTerm = null);
        Task<IEnumerable<Product>> GetOutOfStockProductsAsync();
        Task<IEnumerable<Product>> GetLowStockAlertsAsync(int threshold);
        Task UpdateProductStatusAsync(Guid productId, bool isActive);
        Task<IEnumerable<Product>> GetBestSellersAsync(int topCount);
        Task<decimal> GetTotalInventoryValueAsync();
        Task<bool> IsProductNameUniqueAsync(string name);

        Task<IEnumerable<Product>> GetDeletedProductsAsync();
        Task<Product?> GetByIdDeletedAsync(Guid id);
    }
}
