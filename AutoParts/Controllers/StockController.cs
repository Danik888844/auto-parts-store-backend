using AutoParts.Business.Cqrs.Stocks;
using AutoParts.DataAccess.Models.DtoModels.Stock;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockController : BaseController
{
    private readonly IMediator _mediator;

    public StockController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("by-product/{productId}")]
    public async Task<IActionResult> GetByProduct(int productId)
    {
        return Return(await _mediator.Send(new StockGetByProductCommand(productId)));
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetList(StockListFormDto form)
    {
        return Return(await _mediator.Send(new StockGetListCommand(form)));
    }
}
