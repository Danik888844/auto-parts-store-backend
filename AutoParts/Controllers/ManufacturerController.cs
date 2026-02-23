using AutoParts.Business.Cqrs.Manufacturers;
using AutoParts.DataAccess.Models.DtoModels;
using AutoParts.DataAccess.Models.DtoModels.Manufacturer;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ManufacturerController : BaseController
{
    #region DI

    private readonly IMediator _mediator;

    #endregion

    public ManufacturerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        return Return(await _mediator.Send(new ManufacturerGetByCommand(id)));
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetList(PaginationFormDto form)
    {
        return Return(await _mediator.Send(new ManufacturerGetListCommand(form)));
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Create(ManufacturerFormDto form)
    {
        return Return(await _mediator.Send(new ManufacturerCreateCommand(form)));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Edit([FromRoute] int id, ManufacturerFormDto form)
    {
        return Return(await _mediator.Send(new ManufacturerEditCommand(id, form)));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        return Return(await _mediator.Send(new ManufacturerDeleteCommand(id)));
    }
}
