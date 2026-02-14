namespace AutoParts.DataAccess.Models.DtoModels.Product;

public class ProductListDto
{
    private List<ProductReturnItemDto> Products { get; set; } = [];
}

public class ProductReturnItemDto
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string ManufacturerName { get; set; } = string.Empty;
    public int StockQty { get; set; }
    public bool IsActive { get; set; }
}