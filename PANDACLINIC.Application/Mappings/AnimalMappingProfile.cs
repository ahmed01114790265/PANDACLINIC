using Mapster;
using PANDACLINIC.Application.DTOS.Animal;
using PANDACLINIC.Domain.Entity;
using System.Linq;
using System.Reflection;

namespace PANDACLINIC.Application.Mappings
{
    public class AnimalMappingProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Animal, AnimalSummaryDto>();

            config.NewConfig<Animal, AnimalDetailDto>()
                .Map(dest => dest.OwnerName, src => src.User != null ? src.User.fullName : "Unknown")
                .Map(dest => dest.BirthDate, src => src.CreatedAt)
                .Map(dest => dest.AppointmentCount, src => src.Appointments != null ? src.Appointments.Count : 0)
                .Map(dest => dest.IsHostedNow, src => src.HostingHistory != null && src.HostingHistory.Any(h => h.CheckOutDate == null));

            config.NewConfig<AnimalRequestDto, Animal>()
                .Ignore(dest => dest.Id)
                .Map(dest => dest.CreatedAt, src => src.BirthDate)
                .Map(dest => dest.AnimalType, src => src.AnimalType)
                .AfterMapping((src, dest) =>
                {
                    var nameProp = dest.GetType().GetProperty("Name", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    nameProp?.SetValue(dest, src.Name);
                });
        }
    }
}
