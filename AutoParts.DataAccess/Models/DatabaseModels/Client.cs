using System.ComponentModel.DataAnnotations.Schema;

namespace AutoParts.DataAccess.Models.DatabaseModels;

public class Client : BaseEntity
{
    public string FullName { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Notes { get; set; }
    
    [InverseProperty(nameof(Sale.Client))] public List<Sale> Sales { get; set; } = [];
}