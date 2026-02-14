using System.ComponentModel.DataAnnotations.Schema;

namespace AutoParts.DataAccess.Models.DatabaseModels;

public class Supplier : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }

    [InverseProperty(nameof(StockMovement.Supplier))] public List<StockMovement> StockMovements { get; set; } = [];
}