using AutoParts.DataAccess.Models.DtoModels.Product;
using AutoParts.DataAccess.Models.DtoModels.Vehicle;

namespace AutoParts.DataAccess.Models.DtoModels.ProductCompatibility;

public class ProductCompatibilityDto
{
    public int ProductId { get; set; }
    public ProductDto? Product { get; set; }

    public int VehicleId { get; set; }
    public VehicleDto? Vehicle { get; set; } 

    public string? Comment { get; set; }
}