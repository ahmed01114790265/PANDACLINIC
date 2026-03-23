using Mapster;
using PANDACLINIC.Application.DTOS.Appointment;
using PANDACLINIC.Domain.Entity;

namespace PANDACLINIC.Application.Mappings
{
    public class AppointmentMappingProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Appointment, AppointmentSummaryDto>()
                .Map(dest => dest.AnimalName, src => src.Animal != null ? src.Animal.Name : string.Empty)
                .Map(dest => dest.AnimalType, src => src.Animal != null ? src.Animal.AnimalType.ToString() : string.Empty);

            config.NewConfig<Appointment, AppointmentDetailDto>()
                .Map(dest => dest.AnimalName, src => src.Animal != null ? src.Animal.Name : string.Empty)
                .Map(dest => dest.OwnerName, src => src.Animal != null && src.Animal.User != null ? src.Animal.User.fullName : string.Empty)
                .Map(dest => dest.AnimalType, src => src.Animal != null ? src.Animal.AnimalType.ToString() : string.Empty);

            config.NewConfig<AppointmentRequestDto, Appointment>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.Animal);
        }
    }
}
