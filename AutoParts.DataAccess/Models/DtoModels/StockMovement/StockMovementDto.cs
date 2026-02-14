using AutoParts.DataAccess.Models.DtoModels.IdentityModels;
using AutoParts.DataAccess.Models.DtoModels.Product;
using AutoParts.DataAccess.Models.DtoModels.Supplier;
using AutoParts.DataAccess.Models.Enums;

namespace AutoParts.DataAccess.Models.DtoModels.StockMovement;

public class StockMovementDto : DtoBaseEntity
{
    public int ProductId { get; set; }
    public ProductDto Product { get; set; } = null!;

    public StockMovementType Type { get; set; }
    public int Quantity { get; set; }                   // положительное
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

    public string? Reason { get; set; }
    public string? DocumentNo { get; set; }

    public int? SupplierId { get; set; }
    public SupplierDto? Supplier { get; set; }

    public string? UserId { get; set; }
    public UserDto? UserDto { get; set; }
}