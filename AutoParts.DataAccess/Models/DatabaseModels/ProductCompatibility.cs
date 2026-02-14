using System.ComponentModel.DataAnnotations.Schema;

namespace AutoParts.DataAccess.Models.DatabaseModels;

public class ProductCompatibility : BaseEntity
{
    public int ProductId { get; set; }
    
    [ForeignKey("ProductId")]
    public Product Product { get; set; } = null!;

    public int VehicleId { get; set; }
    
    [ForeignKey("VehicleId")]
    public Vehicle Vehicle { get; set; } = null!;

    public string? Comment { get; set; }
}