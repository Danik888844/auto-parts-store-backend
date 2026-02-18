using AutoMapper;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels.Category;
using AutoParts.DataAccess.Models.DtoModels.Manufacturer;
using AutoParts.DataAccess.Models.DtoModels.Client;
using AutoParts.DataAccess.Models.DtoModels.Supplier;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;

namespace AutoParts.Business.ServiceRegistrations;

public static class AutoMapServiceRegistrations
{
    public static IServiceCollection AddAutoMapServiceRegistrations(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddSingleton(new MapperConfiguration(config =>
        {
            config.CreateMap<Category, CategoryDto>().ReverseMap();
            config.CreateMap<Category, CategoryFormDto>().ReverseMap();
            
            config.CreateMap<Manufacturer, ManufacturerDto>().ReverseMap();
            config.CreateMap<Manufacturer, ManufacturerFormDto>().ReverseMap();

            config.CreateMap<Supplier, SupplierDto>().ReverseMap();
            config.CreateMap<Supplier, SupplierFormDto>().ReverseMap();

            config.CreateMap<Client, ClientDto>().ReverseMap();
            config.CreateMap<Client, ClientFormDto>().ReverseMap();
        },
        NullLoggerFactory.Instance).CreateMapper());

        return services;
    }
}