using AutoParts.DataAccess.Models.DtoModels.SaleItem;
using AutoParts.DataAccess.Models.Enums;

namespace AutoParts.DataAccess.Models.DtoModels.Sale;

public class SaleFormDto
{
    public int? ClientId { get; set; }
    public PaymentType PaymentType { get; set; }
    public required List<SaleItemFormDto> Items { get; set; }
}