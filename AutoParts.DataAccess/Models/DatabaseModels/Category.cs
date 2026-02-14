using System.ComponentModel.DataAnnotations.Schema;

namespace AutoParts.DataAccess.Models.DatabaseModels;

public class Category : BaseEntity
{
    public string Name { get; set; } = null!;

    [InverseProperty(nameof(Product.Category))] public List<Product> Products { get; set; } = [];
}