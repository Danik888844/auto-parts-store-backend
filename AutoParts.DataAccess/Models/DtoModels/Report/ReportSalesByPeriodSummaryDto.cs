namespace AutoParts.DataAccess.Models.DtoModels.Report;

/// <summary>
/// Сводка по продажам: сумма, количество чеков, средний чек, количество единиц товара.
/// </summary>
public class ReportSalesByPeriodSummaryDto
{
    /// <summary>Сумма (всего продано на сумму).</summary>
    public decimal TotalAmount { get; set; }

    /// <summary>Количество чеков (продаж).</summary>
    public int ChecksCount { get; set; }

    /// <summary>Средний чек (TotalAmount / ChecksCount или 0).</summary>
    public decimal AverageCheck { get; set; }

    /// <summary>Количество проданных единиц товара (сумма Quantity по позициям).</summary>
    public int ItemsQuantity { get; set; }
}
