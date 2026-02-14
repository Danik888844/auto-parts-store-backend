using AutoParts.DataAccess.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace AutoParts.DataAccess.Contexts;

public class AutoPartsStoreDb : DbContext
{
    public DbSet<Category> Categories {get;set;}
    public DbSet<Client> Clients {get;set;}
    public DbSet<Manufacturer> Manufacturers {get;set;}
    public DbSet<Product> Products {get;set;}
    public DbSet<ProductCompatibility> ProductCompatibilities {get;set;}
    public DbSet<Sale> Sales {get;set;}
    public DbSet<SaleItem> SaleItems {get;set;}
    public DbSet<Stock> Stocks {get;set;}
    public DbSet<StockMovement> StockMovements {get;set;}
    public DbSet<Supplier> Suppliers {get;set;}
    public DbSet<Vehicle> Vehicles {get;set;}
    public DbSet<VehicleBrand> VehicleBrands {get;set;}
    public DbSet<VehicleModel> VehicleModels {get;set;}
    
    public AutoPartsStoreDb(){ }
    
    public AutoPartsStoreDb(DbContextOptions<AutoPartsStoreDb> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Product>()
            .HasIndex(u => u.Sku)
            .IsUnique();
    }
}