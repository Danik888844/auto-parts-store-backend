using AutoParts.DataAccess.Models.Enums;

namespace AutoParts.DataAccess.Models.DtoModels.StockMovement;

public class StockMovementListFormDto
{
    public int PageNumber { get; set; } = 1;
    public int ViewSize { get; set; } = 20;
    public int? ProductId { get; set; }
    public StockMovementType? Type { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}
