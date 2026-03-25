using CalikBackend.Application.DTOs.Products;
using CalikBackend.Application.Features.ProductCategories.Commands.CreateCategory;
using CalikBackend.Application.Features.ProductCategories.Commands.DeleteCategory;
using CalikBackend.Application.Features.ProductCategories.Commands.UpdateCategory;
using CalikBackend.Application.Features.ProductCategories.Queries.GetCategories;
using CalikBackend.Application.Features.ProductCategories.Queries.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalikBackend.API.Controllers;

[ApiController]
[Route("api/product-categories")]
public class ProductCategoriesController : ControllerBase
{
    private readonly ISender _sender;

    public ProductCategoriesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] bool sortDesc = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        => Ok(await _sender.Send(new GetCategoriesQuery(search, sortDesc, page, pageSize)));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => Ok(await _sender.Send(new GetCategoryByIdQuery(id)));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        var result = await _sender.Send(new CreateCategoryCommand(request.Name, request.Description, request.ImageUrl));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request)
        => Ok(await _sender.Send(new UpdateCategoryCommand(id, request.Name, request.Description, request.ImageUrl)));

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sender.Send(new DeleteCategoryCommand(id));
        return NoContent();
    }
}
