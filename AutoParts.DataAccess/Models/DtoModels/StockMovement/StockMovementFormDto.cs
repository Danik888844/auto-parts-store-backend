using AutoParts.DataAccess.Models.Enums;

namespace AutoParts.DataAccess.Models.DtoModels.StockMovement;

public class StockMovementFormDto
{
    public int ProductId { get; set; }
    public StockMovementType Type { get; set; }
    public int Quantity { get; set; }
    public string? Reason { get; set; }
    public string? DocumentNo { get; set; }
    public int? SupplierId { get; set; }
}