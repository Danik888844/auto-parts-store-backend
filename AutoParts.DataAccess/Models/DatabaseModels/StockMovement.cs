using System.ComponentModel.DataAnnotations.Schema;
using AutoParts.DataAccess.Models.Enums;

namespace AutoParts.DataAccess.Models.DatabaseModels;

public class StockMovement : BaseEntity
{
    public int ProductId { get; set; }
    
    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;

    public StockMovementType Type { get; set; }
    public int Quantity { get; set; }                   // положительное
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

    public string? Reason { get; set; }
    public string? DocumentNo { get; set; }

    public int? SupplierId { get; set; }
    
    [ForeignKey("SupplierId")]
    public Supplier? Supplier { get; set; }

    public string? UserId { get; set; }                 // IdentityUser.Id
}