using AutoMapper;
using PANDACLINIC.Application.DTOS.Product;
using PANDACLINIC.Domain.Comman.ValueObject;
using PANDACLINIC.Domain.Entity;

namespace PANDACLINIC.Application.Mappings
{
    public class ProductOrderMappingProfile : Profile
    {
        public ProductOrderMappingProfile()
        {
            CreateMap<Product, ProductSummaryDto>()
                .ForMember(d => d.Price, opt => opt.MapFrom(s => s.Price.Amount))
                .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.Price.Currency));

            CreateMap<Product, ProductDetailDto>()
                .ForMember(d => d.Price, opt => opt.MapFrom(s => s.Price.Amount))
                .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.Price.Currency));

            CreateMap<ProductRequestDto, Product>()
                .ForMember(d => d.Price, opt => opt.MapFrom(s => new Money(s.Price, string.IsNullOrWhiteSpace(s.Currency) ? "EGY" : s.Currency)));
        }
    }
}
