using System.ComponentModel.DataAnnotations.Schema;

namespace AutoParts.DataAccess.Models.DatabaseModels;

public class VehicleModel : BaseEntity
{
    public int BrandId { get; set; }
    
    [ForeignKey("BrandId")]
    public VehicleBrand Brand { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    
    [InverseProperty(nameof(Vehicle.Model))]
    public List<Vehicle> Vehicles { get; set; } = [];
}