using AutoMapper;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels.Category;
using AutoParts.DataAccess.Models.DtoModels.Manufacturer;
using AutoParts.DataAccess.Models.DtoModels.Client;
using AutoParts.DataAccess.Models.DtoModels.Product;
using AutoParts.DataAccess.Models.DtoModels.Supplier;
using AutoParts.DataAccess.Models.DtoModels.ProductCompatibility;
using AutoParts.DataAccess.Models.DtoModels.Vehicle;
using AutoParts.DataAccess.Models.DtoModels.VehicleBrand;
using AutoParts.DataAccess.Models.DtoModels.VehicleModel;
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

            config.CreateMap<Product, ProductDto>().ReverseMap();
            config.CreateMap<Product, ProductFormCreateDto>().ReverseMap();
            
            config.CreateMap<Manufacturer, ManufacturerDto>().ReverseMap();
            config.CreateMap<Manufacturer, ManufacturerFormDto>().ReverseMap();

            config.CreateMap<Supplier, SupplierDto>().ReverseMap();
            config.CreateMap<Supplier, SupplierFormDto>().ReverseMap();

            config.CreateMap<Client, ClientDto>().ReverseMap();
            config.CreateMap<Client, ClientFormDto>().ReverseMap();

            config.CreateMap<VehicleBrand, VehicleBrandDto>().ReverseMap();
            config.CreateMap<VehicleBrand, VehicleBrandFormDto>().ReverseMap();

            config.CreateMap<VehicleModel, VehicleModelDto>().ReverseMap();
            config.CreateMap<VehicleModel, VehicleModelFormDto>().ReverseMap();

            config.CreateMap<Vehicle, VehicleDto>().ReverseMap();
            config.CreateMap<Vehicle, VehicleFormDto>().ReverseMap();

            config.CreateMap<ProductCompatibility, ProductCompatibilityDto>().ReverseMap();
            config.CreateMap<ProductCompatibilityFormDto, ProductCompatibility>();
        },
        NullLoggerFactory.Instance).CreateMapper());

        return services;
    }
}