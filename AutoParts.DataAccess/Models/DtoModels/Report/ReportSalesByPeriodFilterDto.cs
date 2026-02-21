using AutoParts.DataAccess.Models.Enums;

namespace AutoParts.DataAccess.Models.DtoModels.Report;

/// <summary>
/// Фильтр отчёта «Продажи за период».
/// </summary>
public class ReportSalesByPeriodFilterDto
{
    /// <summary>Начало периода (UTC).</summary>
    public DateTime DateFrom { get; set; }

    /// <summary>Конец периода (UTC), не включая.</summary>
    public DateTime DateTo { get; set; }

    /// <summary>Продавец (UserId). null — все.</summary>
    public string? UserId { get; set; }

    /// <summary>Способ оплаты. null — все.</summary>
    public PaymentType? PaymentType { get; set; }

    /// <summary>Как учитывать возвраты.</summary>
    public ReturnsMode ReturnsMode { get; set; } = ReturnsMode.Exclude;

    /// <summary>Группировка по дням или по неделям.</summary>
    public SalesReportGroupBy GroupBy { get; set; } = SalesReportGroupBy.Day;
}
