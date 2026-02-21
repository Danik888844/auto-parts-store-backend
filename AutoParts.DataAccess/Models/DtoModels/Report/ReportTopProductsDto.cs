namespace AutoParts.DataAccess.Models.DtoModels.Report;

/// <summary>
/// Отчёт «Топ товаров за период»: данные для графика и таблица.
/// </summary>
public class ReportTopProductsDto
{
    /// <summary>Массив для графика: { "x": "SKU название", "y": значение (qty или revenue в зависимости от OrderBy) }.</summary>
    public List<ReportTopProductsChartPointDto> ChartData { get; set; } = [];

    /// <summary>Таблица: SKU, name, qty, revenue.</summary>
    public List<ReportTopProductsTableRowDto> Table { get; set; } = [];
}

/// <summary>Точка для графика: ось X (подпись), ось Y (значение).</summary>
public class ReportTopProductsChartPointDto
{
    public string X { get; set; } = "";
    public decimal Y { get; set; }
}

/// <summary>Строка таблицы топа товаров.</summary>
public class ReportTopProductsTableRowDto
{
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public int Qty { get; set; }
    public decimal Revenue { get; set; }
}
