using AutoParts.DataAccess.Models.DtoModels.Product;

namespace AutoParts.DataAccess.Models.DtoModels.Category;

public class CategoryDto : DtoBaseEntity
{
    public string Name { get; set; } = string.Empty;
    public List<ProductDto> Products { get; set; } = [];
}