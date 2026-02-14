using AutoParts.DataAccess.Models.DtoModels.Client;
using AutoParts.DataAccess.Models.DtoModels.IdentityModels;
using AutoParts.DataAccess.Models.DtoModels.SaleItem;
using AutoParts.DataAccess.Models.Enums;

namespace AutoParts.DataAccess.Models.DtoModels.Sale;

public class SaleDto : DtoBaseEntity
{
    public DateTime SoldAt { get; set; } = DateTime.UtcNow;
    
    public int? ClientId { get; set; }
    public ClientDto? Client { get; set; }
    
    public string UserId { get; set; } = null!;         // продавец (Identity)
    public UserDto? User { get; set; }
    
    public PaymentType PaymentType { get; set; }
    public SaleStatus Status { get; set; } = SaleStatus.Completed;

    public decimal Total { get; set; }
    public List<SaleItemDto> Items { get; set; } = [];
}