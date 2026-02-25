using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PANDACLINIC.Application.FileService;
using PANDACLINIC.Application.ImmplementationServices.AnimalService;
using PANDACLINIC.Application.InterfacesService.AnimalService;
using PANDACLINIC.Application.Mappings;

namespace PANDACLINIC.Application
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
         
            services.AddAutoMapper(typeof(AnimalMappingProfile).Assembly);
            services.AddScoped<IAnimalService, AnimalService>();

            var globalUploadsPath = configuration["FileStorage:Path"];
            services.AddScoped<IFileService>(sp => new FileServices(globalUploadsPath));
            return services;

        }
    }
}

