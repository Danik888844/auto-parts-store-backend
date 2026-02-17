using AutoParts.Business.Validators.Category;
using AutoParts.Business.Validators.Manufacturer;
using AutoParts.DataAccess.Models.DtoModels;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AutoParts.Business.ServiceRegistrations;

public static class FluentValidationServiceRegistrations
{
    public static IServiceCollection AddFluentValidationServices(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddValidatorsFromAssemblyContaining<PaginationFormDto>();
        
        #region Category

        services.AddValidatorsFromAssemblyContaining<CategoryCreateFormValidator>();

        #endregion

        #region Manufacturer

        services.AddValidatorsFromAssemblyContaining<ManufacturerCreateFormValidator>();

        #endregion
        
        return services;
    }
}