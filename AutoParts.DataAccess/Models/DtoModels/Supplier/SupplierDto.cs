using AutoParts.DataAccess.Models.DtoModels;
using AutoParts.DataAccess.Models.DtoModels.StockMovement;

namespace AutoParts.DataAccess.Models.DtoModels.Supplier;

public class SupplierDto : DtoBaseEntity
{
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public List<StockMovementDto> StockMovements { get; set; } = [];
}

public class SupplierFormDto
{
    public required string Name { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
}