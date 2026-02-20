using AutoMapper;
using AutoParts.Business.Mappings;
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
            config.AddProfile<VehicleMappingProfile>();
            config.AddProfile<CategoryProductMappingProfile>();
            config.AddProfile<StockMappingProfile>();
            config.AddProfile<SaleMappingProfile>();
            config.AddProfile<CommonMappingProfile>();
        },
        NullLoggerFactory.Instance).CreateMapper());

        return services;
    }
}
