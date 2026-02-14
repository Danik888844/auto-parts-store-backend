using System.ComponentModel.DataAnnotations.Schema;

namespace AutoParts.DataAccess.Models.DatabaseModels;

// авто, под который задаём совместимость
public class Vehicle : BaseEntity
{
    public int ModelId { get; set; }
    
    [ForeignKey("ModelId")]
    public VehicleModel Model { get; set; } = null!;

    public int YearFrom { get; set; }
    public int YearTo { get; set; }

    public string? Generation { get; set; }
    public string? Engine { get; set; }
    public string? BodyType { get; set; }
    public string? Note { get; set; }

    [InverseProperty(nameof(ProductCompatibility.Vehicle))]
    public List<ProductCompatibility> Compatibilities { get; set; } = [];
}