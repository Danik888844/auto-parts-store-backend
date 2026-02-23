using AutoParts.Business.Cqrs.Products;
using AutoParts.DataAccess.Models.DtoModels;
using AutoParts.DataAccess.Models.DtoModels.Product;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductController : BaseController
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("check-sku")]
    public async Task<IActionResult> CheckSku([FromQuery] string sku, [FromQuery] int? excludeId = null)
    {
        return Return(await _mediator.Send(new ProductCheckSkuCommand(sku, excludeId)));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        return Return(await _mediator.Send(new ProductGetByCommand(id)));
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetList(PaginationFormDto form)
    {
        return Return(await _mediator.Send(new ProductGetListCommand(form)));
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Create(ProductFormCreateDto form)
    {
        return Return(await _mediator.Send(new ProductCreateCommand(form)));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Edit([FromRoute] int id, ProductFormUpdateDto form)
    {
        return Return(await _mediator.Send(new ProductEditCommand(id, form)));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        return Return(await _mediator.Send(new ProductDeleteCommand(id)));
    }
}
