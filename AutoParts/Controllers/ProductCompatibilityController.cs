using AutoParts.Business.Cqrs.ProductCompatibilities;
using AutoParts.DataAccess.Models.DtoModels;
using AutoParts.DataAccess.Models.DtoModels.ProductCompatibility;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductCompatibilityController : BaseController
{
    private readonly IMediator _mediator;

    public ProductCompatibilityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        return Return(await _mediator.Send(new ProductCompatibilityGetByCommand(id)));
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetList(PaginationFormDto form)
    {
        return Return(await _mediator.Send(new ProductCompatibilityGetListCommand(form)));
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductCompatibilityFormDto form)
    {
        return Return(await _mediator.Send(new ProductCompatibilityCreateCommand(form)));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Edit([FromRoute] int id, ProductCompatibilityFormDto form)
    {
        return Return(await _mediator.Send(new ProductCompatibilityEditCommand(id, form)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        return Return(await _mediator.Send(new ProductCompatibilityDeleteCommand(id)));
    }
}
