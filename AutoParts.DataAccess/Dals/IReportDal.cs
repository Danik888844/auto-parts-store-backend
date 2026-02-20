using AutoParts.DataAccess.Models.DtoModels.Report;

namespace AutoParts.DataAccess.Dals;

public interface IReportDal
{
    Task<ReportDashboardDto> GetDashboardAsync(DateTime todayStartUtc, DateTime weekStartUtc);
    Task<ReportSalesByMonthDto> GetSalesByMonthAsync(DateTime monthStartUtc, DateTime monthEndUtc);
}
