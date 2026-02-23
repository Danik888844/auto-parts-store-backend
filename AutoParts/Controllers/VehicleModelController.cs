using AutoParts.Business.Cqrs.VehicleModels;
using AutoParts.DataAccess.Models.DtoModels;
using AutoParts.DataAccess.Models.DtoModels.VehicleModel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VehicleModelController : BaseController
{
    private readonly IMediator _mediator;

    public VehicleModelController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        return Return(await _mediator.Send(new VehicleModelGetByCommand(id)));
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetList(PaginationFormDto form)
    {
        return Return(await _mediator.Send(new VehicleModelGetListCommand(form)));
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Create(VehicleModelFormDto form)
    {
        return Return(await _mediator.Send(new VehicleModelCreateCommand(form)));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Edit([FromRoute] int id, VehicleModelFormDto form)
    {
        return Return(await _mediator.Send(new VehicleModelEditCommand(id, form)));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        return Return(await _mediator.Send(new VehicleModelDeleteCommand(id)));
    }
}
