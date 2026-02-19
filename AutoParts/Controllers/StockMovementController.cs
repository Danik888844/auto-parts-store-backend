using System.Security.Claims;
using AutoParts.Business.Cqrs.StockMovements;
using AutoParts.DataAccess.Models.DtoModels.StockMovement;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockMovementController : BaseController
{
    private readonly IMediator _mediator;

    public StockMovementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private static string? GetCurrentUserId(HttpContext httpContext) =>
        httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? httpContext.User?.FindFirst("sub")?.Value;

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        return Return(await _mediator.Send(new StockMovementGetByCommand(id)));
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetList(StockMovementListFormDto form)
    {
        return Return(await _mediator.Send(new StockMovementGetListCommand(form)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StockMovementFormDto form)
    {
        var userId = GetCurrentUserId(HttpContext);
        return Return(await _mediator.Send(new StockMovementCreateCommand(form, userId)));
    }
}
