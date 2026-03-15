using AutoMapper;
using PANDACLINIC.Application.DTOS.Appointment;
using PANDACLINIC.Domain.Entity;
using PANDACLINIC.Shared.Enums;

namespace PANDACLINIC.Application.Mappings
{
    public class AppointmentMappingProfile : Profile
    {
        public AppointmentMappingProfile()
        {
            CreateMap<Appointment, AppointmentSummaryDto>()
                .ForMember(dest => dest.AnimalName,
                    opt => opt.MapFrom(src => src.Animal != null ? src.Animal.Name : string.Empty))
                .ForMember(dest => dest.AnimalType,
            opt => opt.MapFrom(src => src.Animal != null ? src.Animal.AnimalType.ToString() : string.Empty));

            CreateMap<Appointment, AppointmentDetailDto>()
                .ForMember(dest => dest.AnimalName,
                    opt => opt.MapFrom(src => src.Animal != null ? src.Animal.Name : string.Empty))
                .ForMember(dest => dest.OwnerName,
                    opt => opt.MapFrom(src => src.Animal != null && src.Animal.User != null ? src.Animal.User.fullName : string.Empty))
                .ForMember(dest => dest.AnimalType,
            opt => opt.MapFrom(src => src.Animal != null ? src.Animal.AnimalType.ToString() : string.Empty));

            CreateMap<AppointmentRequestDto, Appointment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                
                .ForMember(dest => dest.Animal, opt => opt.Ignore());
        }
    }
}
