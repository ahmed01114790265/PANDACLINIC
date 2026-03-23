using Mapster;
using PANDACLINIC.Application.DTOS.Hosting;
using PANDACLINIC.Domain.Entity;

namespace PANDACLINIC.Application.Mappings
{
    public class HostingMappingProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<HostingStay, HostingSummaryDto>()
                .Map(dest => dest.AnimalName, src => src.Animal != null ? src.Animal.Name : string.Empty)
                .Map(dest => dest.ClientName, src => src.Animal != null && src.Animal.User != null ? src.Animal.User.fullName : string.Empty)
                .Map(dest => dest.ClientPhone, src => src.Animal != null && src.Animal.User != null ? src.Animal.User.PhoneNumber ?? string.Empty : string.Empty);

            config.NewConfig<HostingStay, HostingDetailDto>()
                .Map(dest => dest.AnimalName, src => src.Animal != null ? src.Animal.Name : string.Empty)
                .Map(dest => dest.OwnerName, src => src.Animal != null && src.Animal.User != null ? src.Animal.User.fullName : string.Empty)
                .Map(dest => dest.ClientPhone, src => src.Animal != null && src.Animal.User != null ? src.Animal.User.PhoneNumber ?? string.Empty : string.Empty);

            config.NewConfig<HostingRequestDto, HostingStay>()
                .Ignore(dest => dest.Id);
        }
    }
}
