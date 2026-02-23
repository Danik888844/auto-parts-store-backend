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

    public async Task<ReportSalesByPeriodDto> GetSalesByPeriodAsync(ReportSalesByPeriodFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<Models.DatabaseModels.Sale>()
            .AsNoTracking()
            .Where(s => !s.IsDeleted && s.SoldAt >= filter.DateFrom && s.SoldAt < filter.DateTo);

        if (!string.IsNullOrWhiteSpace(filter.UserId))
            query = query.Where(s => s.UserId == filter.UserId);
        if (filter.PaymentType.HasValue)
            query = query.Where(s => s.PaymentType == filter.PaymentType.Value);

        var sales = query.Select(s => new
        {
            s.SoldAt,
            s.Total,
            s.Status,
            ItemsQuantity = s.Items.Sum(i => i.Quantity)
        });

        if (filter.ReturnsMode == ReturnsMode.Exclude)
            sales = sales.Where(s => s.Status == SaleStatus.Completed);

        var salesList = await sales.ToListAsync(cancellationToken);

        var completed = salesList.Where(s => s.Status == SaleStatus.Completed).ToList();
        var refunded = salesList.Where(s => s.Status == SaleStatus.Refunded).ToList();

        decimal totalAmount;
        int checksCount;
        int itemsQuantity;
        List<decimal[]> data;
        ReportSalesByPeriodSummaryDto? returnsSummary = null;
        List<decimal[]>? returnsData = null;

        switch (filter.ReturnsMode)
        {
            case ReturnsMode.Exclude:
                totalAmount = completed.Sum(s => s.Total);
                checksCount = completed.Count;
                itemsQuantity = completed.Sum(s => s.ItemsQuantity);
                data = BuildGroupedData(completed, filter.GroupBy, x => x.SoldAt, x => x.Total);
                break;
            case ReturnsMode.Include:
                totalAmount = completed.Sum(s => s.Total) - refunded.Sum(s => s.Total);
                checksCount = completed.Count + refunded.Count;
                itemsQuantity = completed.Sum(s => s.ItemsQuantity) - refunded.Sum(s => s.ItemsQuantity);
                data = BuildGroupedDataCombined(
                    completed.Select(s => (s.SoldAt, s.Total)).ToList(),
                    refunded.Select(s => (s.SoldAt, s.Total)).ToList(),
                    filter.GroupBy);
                break;
            default: // Separate
                totalAmount = completed.Sum(s => s.Total);
                checksCount = completed.Count;
                itemsQuantity = completed.Sum(s => s.ItemsQuantity);
                data = BuildGroupedData(completed, filter.GroupBy, x => x.SoldAt, x => x.Total);
                returnsSummary = new ReportSalesByPeriodSummaryDto
                {
                    TotalAmount = refunded.Sum(s => s.Total),
                    ChecksCount = refunded.Count,
                    AverageCheck = refunded.Count > 0 ? refunded.Sum(s => s.Total) / refunded.Count : 0,
                    ItemsQuantity = refunded.Sum(s => s.ItemsQuantity)
                };
                returnsData = BuildGroupedData(refunded, filter.GroupBy, x => x.SoldAt, x => x.Total);
                break;
        }

        var summary = new ReportSalesByPeriodSummaryDto
        {
            TotalAmount = totalAmount,
            ChecksCount = checksCount,
            AverageCheck = checksCount > 0 ? totalAmount / checksCount : 0,
            ItemsQuantity = itemsQuantity
        };

        return new ReportSalesByPeriodDto
        {
            Summary = summary,
            Data = data,
            ReturnsSummary = returnsSummary,
            ReturnsData = returnsData
        };
    }

    public async Task<List<ReportSaleRowDto>> GetSalesForPeriodExportAsync(ReportSalesByPeriodFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<Models.DatabaseModels.Sale>()
            .AsNoTracking()
            .Where(s => !s.IsDeleted && s.SoldAt >= filter.DateFrom && s.SoldAt < filter.DateTo);

        if (!string.IsNullOrWhiteSpace(filter.UserId))
            query = query.Where(s => s.UserId == filter.UserId);
        if (filter.PaymentType.HasValue)
            query = query.Where(s => s.PaymentType == filter.PaymentType.Value);
        // При Exclude — только завершённые; при Include/Separate — все продажи и возвраты (в таблице виден статус).
        if (filter.ReturnsMode == ReturnsMode.Exclude)
            query = query.Where(s => s.Status == SaleStatus.Completed);

        var list = await query
            .Select(s => new ReportSaleRowDto
            {
                Id = s.Id,
                SoldAt = s.SoldAt,
                SellerName = s.UserId,
                PaymentType = s.PaymentType,
                Status = s.Status,
                Total = s.Total,
                ItemsQuantity = s.Items.Sum(i => i.Quantity)
            })
            .OrderBy(s => s.SoldAt)
            .ToListAsync(cancellationToken);

        return list;
    }

    private static List<decimal[]> BuildGroupedData<T>(
        List<T> items,
        SalesReportGroupBy groupBy,
        Func<T, DateTime> dateSelector,
        Func<T, decimal> totalSelector)
    {
        var keySelector = groupBy == SalesReportGroupBy.Week ? GetWeekStart : (Func<DateTime, DateTime>)GetDayStart;
        var grouped = items
            .GroupBy(x => keySelector(dateSelector(x)))
            .Select(g => new { Key = g.Key, Total = g.Sum(totalSelector) })
            .OrderBy(x => x.Key)
            .ToList();

        return grouped
            .Select(x =>
            {
                var dateMs = new DateTimeOffset(x.Key.Year, x.Key.Month, x.Key.Day, 0, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds();
                return new[] { (decimal)dateMs, x.Total };
            })
            .ToList();
    }

    private static DateTime GetDayStart(DateTime d) => d.Date;
    private static DateTime GetWeekStart(DateTime d)
    {
        var offset = (int)d.DayOfWeek - (int)DayOfWeek.Monday;
        if (offset < 0) offset += 7;
        return d.Date.AddDays(-offset);
    }

    private static List<decimal[]> BuildGroupedDataCombined(
        List<(DateTime SoldAt, decimal Total)> completed,
        List<(DateTime SoldAt, decimal Total)> refunded,
        SalesReportGroupBy groupBy)
    {
        var dayOrWeek = groupBy == SalesReportGroupBy.Week;
        var completedByKey = completed
            .GroupBy(s => dayOrWeek ? GetWeekStart(s.SoldAt) : GetDayStart(s.SoldAt))
            .ToDictionary(g => g.Key, g => g.Sum(s => s.Total));
        var refundedByKey = refunded
            .GroupBy(s => dayOrWeek ? GetWeekStart(s.SoldAt) : GetDayStart(s.SoldAt))
            .ToDictionary(g => g.Key, g => g.Sum(s => s.Total));

        var allKeys = completedByKey.Keys.Union(refundedByKey.Keys).Distinct().OrderBy(x => x).ToList();
        return allKeys
            .Select(key =>
            {
                var net = (completedByKey.TryGetValue(key, out var c) ? c : 0) - (refundedByKey.TryGetValue(key, out var r) ? r : 0);
                var dateMs = new DateTimeOffset(key.Year, key.Month, key.Day, 0, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds();
                return new[] { (decimal)dateMs, net };
            })
            .ToList();
    }

    public async Task<ReportTopProductsDto> GetTopProductsAsync(ReportTopProductsFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<Models.DatabaseModels.SaleItem>()
            .AsNoTracking()
            .Where(si => !si.IsDeleted
                && si.Sale != null && !si.Sale.IsDeleted && si.Sale.Status == SaleStatus.Completed
                && si.Sale.SoldAt >= filter.DateFrom && si.Sale.SoldAt < filter.DateTo
                && si.Product != null && !si.Product.IsDeleted);

        if (filter.CategoryId.HasValue)
            query = query.Where(si => si.Product!.CategoryId == filter.CategoryId.Value);
        if (filter.ManufacturerId.HasValue)
            query = query.Where(si => si.Product!.ManufacturerId == filter.ManufacturerId.Value);

        var grouped = query
            .GroupBy(si => new { si.ProductId, si.Product!.Sku, si.Product.Name })
            .Select(g => new ReportTopProductsTableRowDto
            {
                Sku = g.Key.Sku,
                Name = g.Key.Name,
                Qty = g.Sum(si => si.Quantity),
                Revenue = g.Sum(si => si.LineTotal)
            });

        var ordered = filter.OrderBy == TopProductsOrderBy.ByRevenue
            ? grouped.OrderByDescending(r => r.Revenue)
            : grouped.OrderByDescending(r => r.Qty);

        var table = await ordered.ToListAsync(cancellationToken);

        return new ReportTopProductsDto { Table = table };
    }
}
