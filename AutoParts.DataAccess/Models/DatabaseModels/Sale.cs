using System.ComponentModel.DataAnnotations.Schema;
using AutoParts.DataAccess.Models.Enums;

namespace AutoParts.DataAccess.Models.DatabaseModels;

public class Sale : BaseEntity
{
    public DateTime SoldAt { get; set; } = DateTime.UtcNow;

    
    public int? ClientId { get; set; }
    
    [ForeignKey("ClientId")]
    public Client? Client { get; set; }

    
    public string UserId { get; set; } = null!;         // продавец (Identity)
    public PaymentType PaymentType { get; set; }
    public SaleStatus Status { get; set; } = SaleStatus.Completed;

    public decimal Total { get; set; }

    [InverseProperty(nameof(SaleItem.Sale))]
    public List<SaleItem> Items { get; set; } = [];
}