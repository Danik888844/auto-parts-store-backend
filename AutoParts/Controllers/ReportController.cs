using AutoParts.Business.Cqrs.Reports;
using AutoParts.DataAccess.Models.DtoModels.Report;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportController : BaseController
{
    private readonly IMediator _mediator;

    public ReportController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Дашборд: продажи за сегодня, доход за неделю, остатки, клиенты, продажи по дням за текущий месяц.
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        return Return(await _mediator.Send(new ReportDashboardCommand()));
    }

    /// <summary>
    /// Отчёт «Продажи за период»: сводка (сумма, кол-во чеков, средний чек, кол-во единиц) и график по дням/неделям.
    /// Фильтры: период (DateFrom, DateTo), продавец (UserId), способ оплаты (PaymentType), учёт возвратов (ReturnsMode), группировка (GroupBy: Day/Week).
    /// </summary>
    [HttpGet("sales-by-period")]
    public async Task<IActionResult> SalesByPeriod([FromQuery] ReportSalesByPeriodFilterDto filter)
    {
        return Return(await _mediator.Send(new ReportSalesByPeriodCommand(filter)));
    }

    /// <summary>
    /// Экспорт отчёта «Продажи за период» в XLSX. Те же фильтры, что и у sales-by-period.
    /// </summary>
    [HttpGet("sales-by-period/export")]
    public async Task<IActionResult> SalesByPeriodExport([FromQuery] ReportSalesByPeriodFilterDto filter)
    {
        var result = await _mediator.Send(new ReportSalesByPeriodExportCommand(filter));
        if (!result.Result)
            return Return(result);
        const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var fileName = $"sales-by-period_{DateTime.UtcNow:yyyy-MM-dd_HH-mm}.xlsx";
        return File(result.Data!, contentType, fileName);
    }

    /// <summary>
    /// Топ товаров за период. Фильтры: период (DateFrom, DateTo), категория (CategoryId), производитель (ManufacturerId), сортировка (OrderBy: 0 — по количеству, 1 — по выручке).
    /// Возвращает ChartData (массив { "x": "SKU название", "y": значение }) и Table (массив { Sku, Name, Qty, Revenue }).
    /// </summary>
    [HttpGet("top-products")]
    public async Task<IActionResult> TopProducts([FromQuery] ReportTopProductsFilterDto filter)
    {
        return Return(await _mediator.Send(new ReportTopProductsCommand(filter)));
    }
}
