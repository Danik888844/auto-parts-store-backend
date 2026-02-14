using AutoParts.DataAccess.Models.DtoModels.Vehicle;
using AutoParts.DataAccess.Models.DtoModels.VehicleBrand;

namespace AutoParts.DataAccess.Models.DtoModels.VehicleModel;

public class VehicleModelDto : DtoBaseEntity
{
    public int BrandId { get; set; }
    public VehicleBrandDto? Brand { get; set; }
    
    public string Name { get; set; } = string.Empty;
    public List<VehicleDto> Vehicles { get; set; } = [];
}