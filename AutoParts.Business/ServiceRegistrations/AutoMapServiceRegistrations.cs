using AutoMapper;
using AutoParts.DataAccess.Models.DatabaseModels;
using AutoParts.DataAccess.Models.DtoModels.Category;
using AutoParts.DataAccess.Models.DtoModels.Manufacturer;
using AutoParts.DataAccess.Models.DtoModels.Client;
using AutoParts.DataAccess.Models.DtoModels.Product;
using AutoParts.DataAccess.Models.DtoModels.Supplier;
using AutoParts.DataAccess.Models.DtoModels.ProductCompatibility;
using AutoParts.DataAccess.Models.DtoModels.Sale;
using AutoParts.DataAccess.Models.DtoModels.SaleItem;
using AutoParts.DataAccess.Models.DtoModels.Stock;
using AutoParts.DataAccess.Models.DtoModels.StockMovement;
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

            config.CreateMap<Stock, StockDto>()
                .ForMember(d => d.Product, o => o.MapFrom(s => s.Product != null ? new ProductDto
                {
                    Id = s.Product.Id,
                    Sku = s.Product.Sku,
                    Name = s.Product.Name,
                    CategoryId = s.Product.CategoryId,
                    ManufacturerId = s.Product.ManufacturerId,
                    Price = s.Product.Price,
                    PurchasePrice = s.Product.PurchasePrice,
                    Description = s.Product.Description,
                    IsActive = s.Product.IsActive,
                    CreatedDate = s.Product.CreatedDate,
                    ModifiedDate = s.Product.ModifiedDate,
                    IsDeleted = s.Product.IsDeleted
                } : null));
            config.CreateMap<StockMovement, StockMovementDto>().ReverseMap();
            config.CreateMap<StockMovementFormDto, StockMovement>();

            config.CreateMap<Sale, SaleDto>().ReverseMap();
            config.CreateMap<SaleItem, SaleItemDto>()
                .ForMember(d => d.ProductSku, o => o.MapFrom(s => s.Product != null ? s.Product.Sku : ""))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product != null ? s.Product.Name : ""));
        },
        NullLoggerFactory.Instance).CreateMapper());

        return services;
    }
}