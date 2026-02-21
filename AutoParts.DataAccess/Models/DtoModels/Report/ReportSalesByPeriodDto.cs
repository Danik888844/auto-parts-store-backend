namespace AutoParts.DataAccess.Models.DtoModels.Report;

/// <summary>
/// Отчёт «Продажи за период»: сводка, график по дням/неделям, при ReturnsMode.Separate — отдельно возвраты.
/// </summary>
public class ReportSalesByPeriodDto
{
    /// <summary>Сводка по продажам (с учётом выбранного ReturnsMode).</summary>
    public ReportSalesByPeriodSummaryDto Summary { get; set; } = new();

    /// <summary>График: пары [дата в миллисекундах (Unix), сумма за период]. Аналог SalesByMonth.</summary>
    public List<decimal[]> Data { get; set; } = [];

    /// <summary>Сводка по возвратам. Заполняется только при ReturnsMode.Separate.</summary>
    public ReportSalesByPeriodSummaryDto? ReturnsSummary { get; set; }

    /// <summary>График по возвратам по дням/неделям. Заполняется только при ReturnsMode.Separate.</summary>
    public List<decimal[]>? ReturnsData { get; set; }
}
