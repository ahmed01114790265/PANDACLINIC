using PANDACLINIC.Application.Mapping;
using PANDACLINIC.Application.DTOS.Product;
using PANDACLINIC.Application.FileService;
using PANDACLINIC.Application.InterfacesService.ProductService;
using PANDACLINIC.Domain.Comman.ValueObject;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Domain.InterfaceRepository;
using PANDACLINIC.Shared.Enums;
using PANDACLINIC.Shared.ResultModel;

namespace PANDACLINIC.Application.ImmplementationServices.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public ProductService(IUnitOfWork uow, IMapper mapper, IFileService fileService)
        {
            _uow = uow;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<Result<IEnumerable<ProductSummaryDto>>> GetStoreProductsAsync(string? search = null, ProductType? type = null)
        {
            IEnumerable<Product> products;

            if (!string.IsNullOrWhiteSpace(search))
                products = await _uow.Products.SearchAsync(search);
            else if (type.HasValue)
                products = await _uow.Products.GetByTypeAsync(type.Value);
            else
                products = await _uow.Products.GetInStockProductsAsync();

            products = products.Where(p => p.IsActive);
            return Result<IEnumerable<ProductSummaryDto>>.Success(_mapper.Map<IEnumerable<ProductSummaryDto>>(products));
        }

        public async Task<Result<IEnumerable<ProductSummaryDto>>> GetAllAsync()
        {
            var products = await _uow.Products.GetAllAsync();
            return Result<IEnumerable<ProductSummaryDto>>.Success(_mapper.Map<IEnumerable<ProductSummaryDto>>(products));
        }

        public async Task<Result<ProductDetailDto>> GetByIdAsync(Guid id)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product == null)
                return Result<ProductDetailDto>.Failure("Product not found.");

            return Result<ProductDetailDto>.Success(_mapper.Map<ProductDetailDto>(product));
        }

        public async Task<Result<ProductDetailDto>> CreateAsync(ProductRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result<ProductDetailDto>.Failure("Product name is required.");

            if (dto.Price <= 0)
                return Result<ProductDetailDto>.Failure("Product price must be greater than zero.");

            if (dto.Stock < 0)
                return Result<ProductDetailDto>.Failure("Stock cannot be negative.");

            var unique = await _uow.Products.IsProductNameUniqueAsync(dto.Name.Trim());
            if (!unique)
                return Result<ProductDetailDto>.Failure("Product name already exists.");

            if (dto.ImageFile != null)
            {
                dto.ImageUrl = await _fileService.UploadFileAsync(dto.ImageFile, "products");
            }

            var entity = _mapper.Map<Product>(dto);
            entity.Price = new Money(dto.Price, string.IsNullOrWhiteSpace(dto.Currency) ? "EGY" : dto.Currency.Trim().ToUpperInvariant());

            if (string.IsNullOrWhiteSpace(entity.ImageUrl))
                entity.ImageUrl = "default-product.png";

            await _uow.Products.AddAsync(entity);
            await _uow.CompleteAsync();

            return Result<ProductDetailDto>.Success(_mapper.Map<ProductDetailDto>(entity));
        }

        public async Task<Result> UpdateAsync(Guid id, ProductRequestDto dto)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product == null)
                return Result.Failure("Product not found.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return Result.Failure("Product name is required.");

            if (dto.Price <= 0)
                return Result.Failure("Product price must be greater than zero.");

            if (dto.Stock < 0)
                return Result.Failure("Stock cannot be negative.");

            var currentImage = product.ImageUrl;
            if (dto.ImageFile != null)
            {
                dto.ImageUrl = await _fileService.UploadFileAsync(dto.ImageFile, "products");
                if (!string.IsNullOrWhiteSpace(currentImage) && currentImage.StartsWith("/uploads/"))
                {
                    _fileService.DeleteFile(currentImage);
                }
            }
            else if (string.IsNullOrWhiteSpace(dto.ImageUrl))
            {
                dto.ImageUrl = currentImage;
            }

            product.Name = dto.Name.Trim();
            product.Description = dto.Description?.Trim() ?? string.Empty;
            product.Price = new Money(dto.Price, string.IsNullOrWhiteSpace(dto.Currency) ? "EGY" : dto.Currency.Trim().ToUpperInvariant());
            product.Weight = dto.Weight;
            product.Taste = dto.Taste?.Trim() ?? string.Empty;
            product.ImageUrl = string.IsNullOrWhiteSpace(dto.ImageUrl) ? "default-product.png" : dto.ImageUrl.Trim();
            product.DiscountPercentage = dto.DiscountPercentage;
            product.Stock = dto.Stock;
            product.Type = dto.Type;
            product.IsActive = dto.IsActive;

            _uow.Products.Update(product);
            await _uow.CompleteAsync();

            return Result.Success("Product updated successfully.");
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product == null)
                return Result.Failure("Product not found.");

            _uow.Products.Delete(product);
            await _uow.CompleteAsync();
            return Result.Success("Product moved to archive successfully.");
        }

        public async Task<Result> ToggleActiveAsync(Guid id, bool isActive)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product == null)
                return Result.Failure("Product not found.");

            product.IsActive = isActive;
            _uow.Products.Update(product);
            await _uow.CompleteAsync();
            return Result.Success("Product status updated successfully.");
        }

        public async Task<Result<IEnumerable<ProductSummaryDto>>> GetDeletedProductsAsync()
        {
            var products = await _uow.Products.GetDeletedProductsAsync();
            return Result<IEnumerable<ProductSummaryDto>>.Success(_mapper.Map<IEnumerable<ProductSummaryDto>>(products));
        }

        public async Task<Result> RestoreAsync(Guid id)
        {
            var product = await _uow.Products.GetByIdDeletedAsync(id);
            if (product == null)
                return Result.Failure("Product not found in archive.");

            product.IsDeleted = false;
            product.DeletedAt = null;
            await _uow.CompleteAsync();
            return Result.Success("Product restored successfully.");
        }
    }
}


