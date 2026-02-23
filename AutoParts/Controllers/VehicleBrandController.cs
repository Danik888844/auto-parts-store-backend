using AutoParts.Business.Cqrs.VehicleBrands;
using AutoParts.DataAccess.Models.DtoModels;
using AutoParts.DataAccess.Models.DtoModels.VehicleBrand;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VehicleBrandController : BaseController
{
    private readonly IMediator _mediator;

    public VehicleBrandController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        return Return(await _mediator.Send(new VehicleBrandGetByCommand(id)));
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetList(PaginationFormDto form)
    {
        return Return(await _mediator.Send(new VehicleBrandGetListCommand(form)));
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Create(VehicleBrandFormDto form)
    {
        return Return(await _mediator.Send(new VehicleBrandCreateCommand(form)));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Edit([FromRoute] int id, VehicleBrandFormDto form)
    {
        return Return(await _mediator.Send(new VehicleBrandEditCommand(id, form)));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        return Return(await _mediator.Send(new VehicleBrandDeleteCommand(id)));
    }
}
