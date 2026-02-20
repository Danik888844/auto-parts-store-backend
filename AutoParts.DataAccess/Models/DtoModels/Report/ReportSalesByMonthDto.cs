namespace AutoParts.DataAccess.Models.DtoModels.Report;

/// <summary>
/// Отчёт: продажи по дням за месяц.
/// Data — пары [дата в миллисекундах (Unix), сумма за день]. Для JSON: [[dateMs, value], ...].
/// </summary>
public class ReportSalesByMonthDto
{
    /// <summary>Пары [дата в миллисекундах, сумма продаж за день].</summary>
    public List<decimal[]> Data { get; set; } = [];
}
