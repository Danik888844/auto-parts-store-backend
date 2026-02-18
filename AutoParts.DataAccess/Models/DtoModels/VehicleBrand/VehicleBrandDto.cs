using AutoParts.DataAccess.Models.DtoModels.VehicleModel;

namespace AutoParts.DataAccess.Models.DtoModels.VehicleBrand;

public class VehicleBrandDto : DtoBaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    public List<VehicleModelDto> Models { get; set; } = [];
}

public class VehicleBrandFormDto
{
    public required string Name { get; set; }
}