using AutoParts.Business.Cqrs.Suppliers;
using AutoParts.DataAccess.Models.DtoModels;
using AutoParts.DataAccess.Models.DtoModels.Supplier;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SupplierController : BaseController
{
    #region DI

    private readonly IMediator _mediator;

    #endregion

    public SupplierController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        return Return(await _mediator.Send(new SupplierGetByCommand(id)));
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetList(PaginationFormDto form)
    {
        return Return(await _mediator.Send(new SupplierGetListCommand(form)));
    }

    [HttpPost]
    public async Task<IActionResult> Create(SupplierFormDto form)
    {
        return Return(await _mediator.Send(new SupplierCreateCommand(form)));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Edit([FromRoute] int id, SupplierFormDto form)
    {
        return Return(await _mediator.Send(new SupplierEditCommand(id, form)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        return Return(await _mediator.Send(new SupplierDeleteCommand(id)));
    }
}
