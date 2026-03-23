using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PANDACLINIC.Application.FileService;
using PANDACLINIC.Application.ImmplementationServices.AnimalService;
using PANDACLINIC.Application.ImmplementationServices.AppointmentService;
using PANDACLINIC.Application.ImmplementationServices.HostingService;
using PANDACLINIC.Application.ImmplementationServices.OrderService;
using PANDACLINIC.Application.ImmplementationServices.ProductService;
using PANDACLINIC.Application.InterfacesService.AnimalService;
using PANDACLINIC.Application.InterfacesService.AppointmentService;
using PANDACLINIC.Application.InterfacesService.HostingService;
using PANDACLINIC.Application.InterfacesService.OrderService;
using PANDACLINIC.Application.InterfacesService.ProductService;
using PANDACLINIC.Application.Mappings;
using PANDACLINIC.Application.Mapping;
using System.Reflection;

namespace PANDACLINIC.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();

            services.AddScoped<IAnimalService, AnimalService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IHostingService, HostingService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();

            var globalUploadsPath = configuration["FileStorage:Path"] ?? string.Empty;
            services.AddScoped<IFileService>(sp => new FileServices(globalUploadsPath));
            return services;
        }
    }
}
