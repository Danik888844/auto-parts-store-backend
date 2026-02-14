using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AutoParts.DataAccess.Models.DatabaseModels;

[Index(nameof(Sku))]
[Index(nameof(Name))]
public class Product : BaseEntity
{
    public string Sku { get; set; } = null!;          // уникально, Stock Keeping Unit - единица складского учета
    public string Name { get; set; } = null!;
    
    
    public int CategoryId { get; set; }
    
    [ForeignKey("CategoryId")]
    public Category Category { get; set; } = null!;
    
    
    public int ManufacturerId { get; set; }
    
    [ForeignKey("ManufacturerId")]
    public Manufacturer Manufacturer { get; set; } = null!;
    

    public decimal? PurchasePrice { get; set; }       // цена на момент покупки
    public decimal Price { get; set; }                // цена продажи

    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public Stock? Stock { get; set; }
    
    [InverseProperty(nameof(ProductCompatibility.Product))]
    public List<ProductCompatibility> Compatibilities { get; set; } = [];
    
    [InverseProperty(nameof(SaleItem.Product))]
    public List<SaleItem> SaleItems { get; set; } = [];
    
    [InverseProperty(nameof(StockMovement.Product))]
    public List<StockMovement> StockMovements { get; set; } = [];
}