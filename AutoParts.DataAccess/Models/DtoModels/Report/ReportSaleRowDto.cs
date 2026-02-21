using AutoParts.DataAccess.Models.Enums;

namespace AutoParts.DataAccess.Models.DtoModels.Report;

/// <summary>
/// Одна строка продажи для экспорта в XLSX.
/// </summary>
public class ReportSaleRowDto
{
    public int Id { get; set; }
    public DateTime SoldAt { get; set; }
    public string? SellerName { get; set; }
    public PaymentType PaymentType { get; set; }
    public SaleStatus Status { get; set; }
    public decimal Total { get; set; }
    public int ItemsQuantity { get; set; }
}
