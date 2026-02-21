using AutoParts.DataAccess.Models.DtoModels.Report;

namespace AutoParts.DataAccess.Dals;

public interface IReportDal
{
    Task<ReportDashboardDto> GetDashboardAsync(DateTime todayStartUtc, DateTime weekStartUtc);
    Task<ReportSalesByMonthDto> GetSalesByMonthAsync(DateTime monthStartUtc, DateTime monthEndUtc);
    Task<ReportSalesByPeriodDto> GetSalesByPeriodAsync(ReportSalesByPeriodFilterDto filter, CancellationToken cancellationToken = default);
    Task<List<ReportSaleRowDto>> GetSalesForPeriodExportAsync(ReportSalesByPeriodFilterDto filter, CancellationToken cancellationToken = default);
    Task<ReportTopProductsDto> GetTopProductsAsync(ReportTopProductsFilterDto filter, CancellationToken cancellationToken = default);
}
