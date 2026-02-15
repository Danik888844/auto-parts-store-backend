namespace AutoParts.DataAccess.Models.DtoModels;

public class PaginationFormDto
{
    public string? Search { get; set; }
    public int ViewSize { get; set; } = 1;
    public int PageNumber { get; set; } = 20;
}