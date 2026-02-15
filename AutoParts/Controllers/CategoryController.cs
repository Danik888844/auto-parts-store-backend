using AutoParts.Business.Cqrs.Categories;
using AutoParts.DataAccess.Models.DtoModels;
using AutoParts.DataAccess.Models.DtoModels.Category;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoParts.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoryController : BaseController
{
    #region DI

    private readonly IMediator _mediator;
    
    #endregion

    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        return Return(await _mediator.Send(new CategoryGetByCommand(id)));
    }
    
    [HttpPost("list")]
    public async Task<IActionResult> GetList(PaginationFormDto form)
    {
        return Return(await _mediator.Send(new CategoryGetListCommand(form)));
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CategoryFormDto form)
    {
        return Return(await _mediator.Send(new CategoryCreateCommand(form)));
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Edit([FromRoute] int id, CategoryFormDto form)
    {
        return Return(await _mediator.Send(new CategoryEditCommand(id, form)));
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        return Return(await _mediator.Send(new CategoryDeleteCommand(id)));
    }
}