using System.ComponentModel.DataAnnotations.Schema;

namespace AutoParts.DataAccess.Models.DatabaseModels;

public class Stock : BaseEntity
{
    public int ProductId { get; set; }
    
    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;
    
    public int Quantity { get; set; }
}