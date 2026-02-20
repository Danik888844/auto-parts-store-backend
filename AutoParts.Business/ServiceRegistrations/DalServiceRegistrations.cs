using AutoParts.DataAccess.Contexts;
using AutoParts.DataAccess.Dals;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AutoParts.Business.ServiceRegistrations;

public static class DalServiceRegistrations
{
    public static IServiceCollection AddDalServices(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment environment)
    {
        #region DbContext

        services.AddDbContext<AutoPartsStoreDb>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("StoreDb"),
                builder => builder.MigrationsAssembly("AutoParts")
            );
        });
        
        #endregion

        #region Dals
        
        services.AddScoped<ICategoryDal, CategoryDal>();
        services.AddScoped<IProductDal, ProductDal>();
        services.AddScoped<IManufacturerDal, ManufacturerDal>();
        services.AddScoped<ISupplierDal, SupplierDal>();
        services.AddScoped<IClientDal, ClientDal>();
        services.AddScoped<IVehicleBrandDal, VehicleBrandDal>();
        services.AddScoped<IVehicleModelDal, VehicleModelDal>();
        services.AddScoped<IVehicleDal, VehicleDal>();
        services.AddScoped<IProductCompatibilityDal, ProductCompatibilityDal>();
        services.AddScoped<IStockDal, StockDal>();
        services.AddScoped<IStockMovementDal, StockMovementDal>();
        services.AddScoped<ISaleDal, SaleDal>();
        services.AddScoped<IReportDal, ReportDal>();

        #endregion

        return services;
    }
}