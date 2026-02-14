using AutoParts.DataAccess.Models.DtoModels.Product;

namespace AutoParts.DataAccess.Models.DtoModels.Stock;

public class StockDto : DtoBaseEntity
{
    public int ProductId { get; set; }
    public ProductDto Product { get; set; } = null!;
    
    public int Quantity { get; set; }
}