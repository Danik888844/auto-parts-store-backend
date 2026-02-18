using AutoParts.Business.Validators.Category;
using AutoParts.Business.Validators.Client;
using AutoParts.Business.Validators.Manufacturer;
using AutoParts.Business.Validators.Product;
using AutoParts.Business.Validators.Supplier;
using AutoParts.Business.Validators.Vehicle;
using AutoParts.Business.Validators.VehicleBrand;
using AutoParts.Business.Validators.VehicleModel;
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

        #region Product

        services.AddValidatorsFromAssemblyContaining<ProductFormCreateValidator>();
        services.AddValidatorsFromAssemblyContaining<ProductFormUpdateValidator>();

        #endregion

        #region Manufacturer

        services.AddValidatorsFromAssemblyContaining<ManufacturerCreateFormValidator>();

        #endregion

        #region Supplier

        services.AddValidatorsFromAssemblyContaining<SupplierCreateFormValidator>();

        #endregion

        #region Client

        services.AddValidatorsFromAssemblyContaining<ClientCreateFormValidator>();

        #endregion

        #region VehicleBrand

        services.AddValidatorsFromAssemblyContaining<VehicleBrandFormValidator>();

        #endregion

        #region VehicleModel

        services.AddValidatorsFromAssemblyContaining<VehicleModelFormValidator>();

        #endregion

        #region Vehicle

        services.AddValidatorsFromAssemblyContaining<VehicleFormValidator>();

        #endregion
        
        return services;
    }
}