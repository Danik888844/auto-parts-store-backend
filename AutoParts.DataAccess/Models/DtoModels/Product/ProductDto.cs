using AutoParts.DataAccess.Models.DtoModels.Category;
using AutoParts.DataAccess.Models.DtoModels.Manufacturer;
using AutoParts.DataAccess.Models.DtoModels.ProductCompatibility;
using AutoParts.DataAccess.Models.DtoModels.SaleItem;
using AutoParts.DataAccess.Models.DtoModels.Stock;
using AutoParts.DataAccess.Models.DtoModels.StockMovement;

namespace AutoParts.DataAccess.Models.DtoModels.Product;

public class ProductDto : DtoBaseEntity
{
    public string Sku { get; set; } = null!;          // уникально, Stock Keeping Unit - единица складского учета
    public string Name { get; set; } = null!;
    
    public int CategoryId { get; set; }
    public CategoryDto Category { get; set; } = null!;
    
    public int ManufacturerId { get; set; }
    public ManufacturerDto Manufacturer { get; set; } = null!;
    
    public decimal? PurchasePrice { get; set; }       // цена на момент покупки
    public decimal Price { get; set; }                // цена продажи

    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public StockDto? Stock { get; set; }
    
    public List<ProductCompatibilityDto> Compatibilities { get; set; } = [];
    public List<SaleItemDto> SaleItems { get; set; } = [];
    public List<StockMovementDto> StockMovements { get; set; } = [];
}