using AutoMapper;
using PANDACLINIC.Application.DTOS.Hosting;
using PANDACLINIC.Domain.Entity;

namespace PANDACLINIC.Application.Mappings
{
    public class HostingMappingProfile : Profile
    {
        public HostingMappingProfile()
        {
            CreateMap<HostingStay, HostingSummaryDto>()
                .ForMember(dest => dest.AnimalName,
                    opt => opt.MapFrom(src => src.Animal != null ? src.Animal.Name : string.Empty))
                .ForMember(dest => dest.ClientName,
                    opt => opt.MapFrom(src => src.Animal != null && src.Animal.User != null ? src.Animal.User.fullName : string.Empty))
                .ForMember(dest => dest.ClientPhone,
                    opt => opt.MapFrom(src => src.Animal != null && src.Animal.User != null ? src.Animal.User.PhoneNumber ?? string.Empty : string.Empty));

            CreateMap<HostingStay, HostingDetailDto>()
                .ForMember(dest => dest.AnimalName,
                    opt => opt.MapFrom(src => src.Animal != null ? src.Animal.Name : string.Empty))
                .ForMember(dest => dest.OwnerName,
                    opt => opt.MapFrom(src => src.Animal != null && src.Animal.User != null ? src.Animal.User.fullName : string.Empty))
                .ForMember(dest => dest.ClientPhone,
                    opt => opt.MapFrom(src => src.Animal != null && src.Animal.User != null ? src.Animal.User.PhoneNumber ?? string.Empty : string.Empty));

            CreateMap<HostingRequestDto, HostingStay>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
