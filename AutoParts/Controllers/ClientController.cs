using AutoParts.Business.Cqrs.Clients;
using AutoParts.DataAccess.Models.DtoModels;
using AutoParts.DataAccess.Models.DtoModels.Client;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientController : BaseController
{
    #region DI

    private readonly IMediator _mediator;

    #endregion

    public ClientController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        return Return(await _mediator.Send(new ClientGetByCommand(id)));
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetList(PaginationFormDto form)
    {
        return Return(await _mediator.Send(new ClientGetListCommand(form)));
    }

    [HttpPost]
    public async Task<IActionResult> Create(ClientFormDto form)
    {
        return Return(await _mediator.Send(new ClientCreateCommand(form)));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Edit([FromRoute] int id, ClientFormDto form)
    {
        return Return(await _mediator.Send(new ClientEditCommand(id, form)));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        return Return(await _mediator.Send(new ClientDeleteCommand(id)));
    }
}
