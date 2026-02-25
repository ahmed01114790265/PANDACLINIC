using AutoMapper;
using PANDACLINIC.Application.DTOS.Animal;
using PANDACLINIC.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDACLINIC.Application.Mappings
{
    public class AnimalMappingProfile : Profile
    {
        public AnimalMappingProfile()
        {
            // 1. Entity -> Summary DTO (Simple/List view)
            CreateMap<Animal, AnimalSummaryDto>();

            // 2. Entity -> Detail DTO (Full view with complex logic)
            CreateMap<Animal, AnimalDetailDto>()
                // Flatten User.FullName to OwnerName
                .ForMember(dest => dest.OwnerName,
                    opt => opt.MapFrom(src => src.User != null ? src.User.fullName : "Unknown"))
                 .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.CreatedAt))
                // Count the Appointments list
                .ForMember(dest => dest.AppointmentCount,
                    opt => opt.MapFrom(src => src.Appointments != null ? src.Appointments.Count : 0))

                // Logic: Check if any Hosting record has no CheckOutDate
                .ForMember(dest => dest.IsHostedNow,
                    opt => opt.MapFrom(src => src.HostingHistory != null &&
                                              src.HostingHistory.Any(h => h.CheckOutDate == null)));

            // 3. Request DTO -> Entity (For Creating)
            // داخل AnimalMappingProfile
            CreateMap<AnimalRequestDto, Animal>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))
                //.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.BirthDate))
                // تجاهل الملف لأنه لا يوجد حقل له في الـ Entity
                .ForMember(dest => dest.Imgageurl, opt => opt.Ignore())
                // التأكد من نقل المسار النصي
                .ForMember(dest => dest.Imgageurl, opt => opt.MapFrom(src => src.Imgageurl));
            // 4. Mapping for Paged Results (if needed)
            // Note: Map specific items first, then use .Select in service or a custom converter
        }
    }
}

