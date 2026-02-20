using AutoParts.DataAccess.Contexts;
using AutoParts.DataAccess.Models.DtoModels.Report;
using AutoParts.DataAccess.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace AutoParts.DataAccess.Dals;

public class ReportDal : IReportDal
{
    private readonly AutoPartsStoreDb _db;

    public ReportDal(AutoPartsStoreDb db)
    {
        _db = db;
    }

    public async Task<ReportDashboardDto> GetDashboardAsync(DateTime todayStartUtc, DateTime weekStartUtc)
    {
        var todayEndUtc = todayStartUtc.AddDays(1);
        var weekEndUtc = weekStartUtc.AddDays(7);

        var salesToday = await _db.Set<Models.DatabaseModels.Sale>()
            .AsNoTracking()
            .Where(s => !s.IsDeleted && s.Status == SaleStatus.Completed
                && s.SoldAt >= todayStartUtc && s.SoldAt < todayEndUtc)
            .Select(s => s.Total)
            .ToListAsync();

        var weekSales = await _db.Set<Models.DatabaseModels.Sale>()
            .AsNoTracking()
            .Where(s => !s.IsDeleted && s.Status == SaleStatus.Completed
                && s.SoldAt >= weekStartUtc && s.SoldAt < weekEndUtc)
            .SumAsync(s => s.Total);

        var stockStats = await _db.Set<Models.DatabaseModels.Stock>()
            .AsNoTracking()
            .Where(s => !s.IsDeleted)
            .Select(s => s.Quantity)
            .ToListAsync();

        var clientsCount = await _db.Set<Models.DatabaseModels.Client>()
            .AsNoTracking()
            .CountAsync(c => !c.IsDeleted);

        return new ReportDashboardDto
        {
            SalesTodayCount = salesToday.Count,
            SalesTodayTotal = salesToday.Sum(),
            IncomeForWeek = weekSales,
            StockPositionsCount = stockStats.Count,
            StockTotalQuantity = stockStats.Sum(),
            ClientsCount = clientsCount
        };
    }

    public async Task<ReportSalesByMonthDto> GetSalesByMonthAsync(DateTime monthStartUtc, DateTime monthEndUtc)
    {
        var byDay = await _db.Set<Models.DatabaseModels.Sale>()
            .AsNoTracking()
            .Where(s => !s.IsDeleted && s.Status == SaleStatus.Completed
                && s.SoldAt >= monthStartUtc && s.SoldAt < monthEndUtc)
            .GroupBy(s => DateOnly.FromDateTime(s.SoldAt))
            .Select(g => new { Date = g.Key, Total = g.Sum(s => s.Total) })
            .ToListAsync();

        var data = byDay
            .Select(x =>
            {
                var dateMs = new DateTimeOffset(x.Date.Year, x.Date.Month, x.Date.Day, 0, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds();
                return new[] { (decimal)dateMs, x.Total };
            })
            .OrderBy(x => x[0])
            .ToList();

        return new ReportSalesByMonthDto { Data = data };
    }
}
