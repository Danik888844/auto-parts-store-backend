using System.ComponentModel.DataAnnotations.Schema;

namespace AutoParts.DataAccess.Models.DatabaseModels;

public class VehicleBrand : BaseEntity
{
    public string Name { get; set; } = null!;
    
    [InverseProperty(nameof(VehicleModel.Brand))]
    public List<VehicleModel> Models { get; set; } =[];
}