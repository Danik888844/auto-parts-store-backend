using AutoParts.DataAccess.Models.Enums;

namespace AutoParts.DataAccess.Models.DtoModels.Report;

/// <summary>
/// Фильтр отчёта «Топ товаров за период».
/// </summary>
public class ReportTopProductsFilterDto
{
    /// <summary>Начало периода (UTC).</summary>
    public DateTime DateFrom { get; set; }

    /// <summary>Конец периода (UTC), не включая.</summary>
    public DateTime DateTo { get; set; }

    /// <summary>Категория. null — все.</summary>
    public int? CategoryId { get; set; }

    /// <summary>Производитель. null — все.</summary>
    public int? ManufacturerId { get; set; }

    /// <summary>Сортировка: по количеству или по выручке.</summary>
    public TopProductsOrderBy OrderBy { get; set; } = TopProductsOrderBy.ByQuantity;
}
