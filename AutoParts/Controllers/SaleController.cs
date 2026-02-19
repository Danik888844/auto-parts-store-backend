using System.Security.Claims;
using AutoParts.Business.Cqrs.Sales;
using AutoParts.DataAccess.Models.DtoModels;
using AutoParts.DataAccess.Models.DtoModels.Sale;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SaleController : BaseController
{
    private readonly IMediator _mediator;

    public SaleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private static string? GetCurrentUserId(HttpContext httpContext) =>
        httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? httpContext.User?.FindFirst("sub")?.Value;

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        return Return(await _mediator.Send(new SaleGetByCommand(id)));
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetList(PaginationFormDto form)
    {
        return Return(await _mediator.Send(new SaleGetListCommand(form)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SaleFormDto form)
    {
        var userId = GetCurrentUserId(HttpContext);
        return Return(await _mediator.Send(new SaleCreateCommand(form, userId)));
    }

    [HttpPost("{saleId}/refund")]
    public async Task<IActionResult> RefundSale([FromRoute] int saleId)
    {
        var userId = GetCurrentUserId(HttpContext);
        return Return(await _mediator.Send(new RefundSaleCommand(saleId, userId)));
    }

    [HttpPost("{saleId}/items/{saleItemId}/refund")]
    public async Task<IActionResult> RefundSaleItem([FromRoute] int saleId, [FromRoute] int saleItemId, [FromQuery] int quantity = 0)
    {
        var userId = GetCurrentUserId(HttpContext);
        return Return(await _mediator.Send(new RefundSaleItemCommand(saleId, saleItemId, quantity, userId)));
    }
}
