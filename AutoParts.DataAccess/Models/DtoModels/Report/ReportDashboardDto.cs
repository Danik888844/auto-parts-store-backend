namespace AutoParts.DataAccess.Models.DtoModels.Report;

/// <summary>
/// Отчёт для дашборда: продажи за сегодня, доход за неделю, остатки, клиенты.
/// </summary>
public class ReportDashboardDto
{
    /// <summary>Количество продаж за сегодня.</summary>
    public int SalesTodayCount { get; set; }

    /// <summary>Сумма продаж за сегодня.</summary>
    public decimal SalesTodayTotal { get; set; }

    /// <summary>Доход за последние 7 дней (сумма по завершённым продажам).</summary>
    public decimal IncomeForWeek { get; set; }

    /// <summary>Количество позиций на складе (записей Stock).</summary>
    public int StockPositionsCount { get; set; }

    /// <summary>Общее количество единиц на складе (сумма Stock.Quantity).</summary>
    public int StockTotalQuantity { get; set; }

    /// <summary>Количество клиентов.</summary>
    public int ClientsCount { get; set; }

    /// <summary>Продажи по дням за текущий месяц. Пары [дата в мс, сумма].</summary>
    public List<decimal[]> SalesByMonth { get; set; } = [];
}
