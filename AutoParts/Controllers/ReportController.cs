using AutoParts.Business.Cqrs.Reports;
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
}
