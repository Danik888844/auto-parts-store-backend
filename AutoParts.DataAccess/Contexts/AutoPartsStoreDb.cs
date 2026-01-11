using Microsoft.EntityFrameworkCore;

namespace AutoParts.DataAccess.Contexts;

public class AutoPartsStoreDb : DbContext
{
    //public DbSet<Product> Product {get;set;}
    
    public AutoPartsStoreDb(){ }
    
    public AutoPartsStoreDb(DbContextOptions<AutoPartsStoreDb> options) : base(options) { }
}