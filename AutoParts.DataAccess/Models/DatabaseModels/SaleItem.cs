using System.ComponentModel.DataAnnotations.Schema;

namespace AutoParts.DataAccess.Models.DatabaseModels;

public class SaleItem : BaseEntity
{
    public int SaleId { get; set; }
    
    [ForeignKey("SaleId")]
    public Sale Sale { get; set; } = null!;

    
    public int ProductId { get; set; }
    
    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;

    
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal LineTotal { get; set; }
}