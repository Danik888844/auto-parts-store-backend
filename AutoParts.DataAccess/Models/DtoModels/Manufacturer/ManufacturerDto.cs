using AutoParts.DataAccess.Models.DtoModels.Product;

namespace AutoParts.DataAccess.Models.DtoModels.Manufacturer;

public class ManufacturerDto : DtoBaseEntity
{
    public string Name { get; set; } = null!;
    public string? Country { get; set; }
    public List<ProductDto> Products { get; set; } = [];
}