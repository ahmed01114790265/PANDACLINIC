using PANDACLINIC.Application.DTOS.Product;
using PANDACLINIC.Shared.ResultModel;

namespace PANDACLINIC.Application.InterfacesService.ProductService
{
    public interface IProductService
    {
        Task<Result<IEnumerable<ProductSummaryDto>>> GetStoreProductsAsync(string? search = null, Shared.Enums.ProductType? type = null);
        Task<Result<IEnumerable<ProductSummaryDto>>> GetAllAsync();
        Task<Result<ProductDetailDto>> GetByIdAsync(Guid id);
        Task<Result<ProductDetailDto>> CreateAsync(ProductRequestDto dto);
        Task<Result> UpdateAsync(Guid id, ProductRequestDto dto);
        Task<Result> DeleteAsync(Guid id);
        Task<Result> ToggleActiveAsync(Guid id, bool isActive);
        Task<Result<IEnumerable<ProductSummaryDto>>> GetDeletedProductsAsync();
        Task<Result> RestoreAsync(Guid id);
    }
}
