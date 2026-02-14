using AutoParts.DataAccess.Models.DtoModels.StockMovement;

namespace AutoParts.DataAccess.Models.DtoModels.Supplier;

public class SupplierDto
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }

    public List<StockMovementDto> StockMovements { get; set; } = [];
}