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
        services.AddScoped<IManufacturerDal, ManufacturerDal>();
        services.AddScoped<ISupplierDal, SupplierDal>();
        
        #endregion

        return services;
    }
}