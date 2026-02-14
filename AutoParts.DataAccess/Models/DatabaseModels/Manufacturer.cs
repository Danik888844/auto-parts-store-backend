using System.ComponentModel.DataAnnotations.Schema;

namespace AutoParts.DataAccess.Models.DatabaseModels;

public class Manufacturer : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Country { get; set; }

    [InverseProperty(nameof(Product.Manufacturer))] public List<Product> Products { get; set; } = [];
}