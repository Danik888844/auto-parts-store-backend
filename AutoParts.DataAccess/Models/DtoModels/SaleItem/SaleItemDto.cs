namespace AutoParts.DataAccess.Models.DtoModels.SaleItem;

public class SaleItemDto : DtoBaseEntity
{
    public int ProductId { get; set; }
    public string ProductSku { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal LineTotal { get; set; }
}