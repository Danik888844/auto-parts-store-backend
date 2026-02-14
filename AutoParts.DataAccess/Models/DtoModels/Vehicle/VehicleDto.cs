using AutoParts.DataAccess.Models.DtoModels.ProductCompatibility;
using AutoParts.DataAccess.Models.DtoModels.VehicleBrand;
using AutoParts.DataAccess.Models.DtoModels.VehicleModel;

namespace AutoParts.DataAccess.Models.DtoModels.Vehicle;

public class VehicleDto : DtoBaseEntity
{
    public int ModelId { get; set; }
    public VehicleModelDto? Model { get; set; }

    public int YearFrom { get; set; }
    public int YearTo { get; set; }

    public string? Generation { get; set; }
    public string? Engine { get; set; }
    public string? BodyType { get; set; }
    public string? Note { get; set; }

    public List<ProductCompatibilityDto> Compatibilities { get; set; } = [];
}