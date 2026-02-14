namespace AutoParts.DataAccess.Models.DtoModels.Product;

public class ProductFormDto
{
    public required string Sku { get; set; }
    public required string Name { get; set; }
    public int CategoryId { get; set; }
    public int ManufacturerId { get; set; }
    public decimal Price { get; set; }
    public decimal? PurchasePrice { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}