namespace AutoParts.DataAccess.Models.DtoModels.Stock;

public class StockListFormDto
{
    public string? Search { get; set; }
    public int PageNumber { get; set; } = 1;
    public int ViewSize { get; set; } = 20;
}
