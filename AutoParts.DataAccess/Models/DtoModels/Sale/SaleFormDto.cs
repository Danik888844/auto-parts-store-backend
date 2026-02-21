using AutoParts.DataAccess.Models.DtoModels.SaleItem;
using AutoParts.DataAccess.Models.Enums;

namespace AutoParts.DataAccess.Models.DtoModels.Sale;

public class SaleFormDto
{
    /// <summary>
    /// Если true — продажа создаётся как черновик (без списания остатков). Иначе — сразу завершённая (Completed).
    /// </summary>
    public bool CreateAsDraft { get; set; }

    public int? ClientId { get; set; }
    public PaymentType PaymentType { get; set; }
    public required List<SaleItemFormDto> Items { get; set; }
}