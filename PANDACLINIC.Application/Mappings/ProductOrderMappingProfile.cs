using Mapster;
using PANDACLINIC.Application.DTOS.Product;
using PANDACLINIC.Domain.Comman.ValueObject;
using PANDACLINIC.Domain.Entity;

namespace PANDACLINIC.Application.Mappings
{
    public class ProductOrderMappingProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Product, ProductSummaryDto>()
                .Map(d => d.Price, s => s.Price != null ? s.Price.Amount : 0)
                .Map(d => d.Currency, s => s.Price != null ? s.Price.Currency : "EGP");

            config.NewConfig<Product, ProductDetailDto>()
                .Map(d => d.Price, s => s.Price != null ? s.Price.Amount : 0)
                .Map(d => d.Currency, s => s.Price != null ? s.Price.Currency : "EGP");

            config.NewConfig<ProductRequestDto, Product>()
                .Map(d => d.Price, s => new Money(s.Price, string.IsNullOrWhiteSpace(s.Currency) ? "EGP" : s.Currency));
        }
    }
}
