using AutoParts.Business.Cqrs.Categories;
using AutoParts.Business.Cqrs.Clients;
using AutoParts.Business.Cqrs.Manufacturers;
using AutoParts.Business.Cqrs.Products;
using AutoParts.Business.Cqrs.Suppliers;
using AutoParts.Business.Cqrs.VehicleBrands;
using AutoParts.Business.Cqrs.VehicleModels;
using AutoParts.Business.Cqrs.Vehicles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AutoParts.Business.ServiceRegistrations;

public static class MediatrServiceRegistrations
{
    public static IServiceCollection AddMediatrServices(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment environment)
    {
        #region Category

        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(CategoryCreateCommand.CategoryCreateCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(CategoryDeleteCommand.CategoryDeleteCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(CategoryEditCommand.CategoryEditCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(CategoryGetByCommand.CategoryGetByCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(CategoryGetListCommand.CategoryGetListCommandHandler).Assembly));

        #endregion

        #region Product

        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(ProductCreateCommand.ProductCreateCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(ProductDeleteCommand.ProductDeleteCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(ProductEditCommand.ProductEditCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(ProductGetByCommand.ProductGetByCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(ProductGetListCommand.ProductGetListCommandHandler).Assembly));

        #endregion

        #region Manufacturer

        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(ManufacturerCreateCommand.ManufacturerCreateCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(ManufacturerDeleteCommand.ManufacturerDeleteCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(ManufacturerEditCommand.ManufacturerEditCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(ManufacturerGetByCommand.ManufacturerGetByCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(ManufacturerGetListCommand.ManufacturerGetListCommandHandler).Assembly));

        #endregion

        #region Supplier

        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(SupplierCreateCommand.SupplierCreateCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(SupplierDeleteCommand.SupplierDeleteCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(SupplierEditCommand.SupplierEditCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(SupplierGetByCommand.SupplierGetByCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(SupplierGetListCommand.SupplierGetListCommandHandler).Assembly));

        #endregion

        #region VehicleBrand

        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(VehicleBrandCreateCommand.VehicleBrandCreateCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(VehicleBrandDeleteCommand.VehicleBrandDeleteCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(VehicleBrandEditCommand.VehicleBrandEditCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(VehicleBrandGetByCommand.VehicleBrandGetByCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(VehicleBrandGetListCommand.VehicleBrandGetListCommandHandler).Assembly));

        #endregion

        #region VehicleModel

        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(VehicleModelCreateCommand.VehicleModelCreateCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(VehicleModelDeleteCommand.VehicleModelDeleteCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(VehicleModelEditCommand.VehicleModelEditCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(VehicleModelGetByCommand.VehicleModelGetByCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(VehicleModelGetListCommand.VehicleModelGetListCommandHandler).Assembly));

        #endregion

        #region Vehicle

        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(VehicleCreateCommand.VehicleCreateCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(VehicleDeleteCommand.VehicleDeleteCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(VehicleEditCommand.VehicleEditCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(VehicleGetByCommand.VehicleGetByCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(VehicleGetListCommand.VehicleGetListCommandHandler).Assembly));

        #endregion

        #region Client

        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(ClientCreateCommand.ClientCreateCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(ClientDeleteCommand.ClientDeleteCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(ClientEditCommand.ClientEditCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(ClientGetByCommand.ClientGetByCommandHandler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(ClientGetListCommand.ClientGetListCommandHandler).Assembly));

        #endregion
        
        return services;
    }
}