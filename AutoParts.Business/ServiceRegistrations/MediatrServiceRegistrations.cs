using AutoParts.Business.Cqrs.Categories;
using AutoParts.Business.Cqrs.Clients;
using AutoParts.Business.Cqrs.Manufacturers;
using AutoParts.Business.Cqrs.Suppliers;
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