using CalikBackend.Application.DTOs.Products;
using CalikBackend.Application.Features.Products.Commands.CreateProduct;
using CalikBackend.Application.Features.Products.Commands.DeleteProduct;
using CalikBackend.Application.Features.Products.Commands.UpdateProduct;
using CalikBackend.Application.Features.Products.Commands.UpdateStock;
using CalikBackend.Application.Features.Products.Queries.GetProductById;
using CalikBackend.Application.Features.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalikBackend.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? categoryId,
        [FromQuery] string? search,
        [FromQuery] string? brand,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] bool? inStock,
        [FromQuery] string? sortBy,
        [FromQuery] bool sortDesc = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        => Ok(await _sender.Send(new GetProductsQuery(
            categoryId, search, brand, minPrice, maxPrice, inStock,
            sortBy, sortDesc, page, pageSize, CanSeePrice())));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => Ok(await _sender.Send(new GetProductByIdQuery(id, CanSeePrice())));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        var result = await _sender.Send(new CreateProductCommand(
            request.Name, request.Description, request.Brand, request.Model,
            request.ImageUrl, request.Price, request.Stock, request.Unit, request.CategoryId));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request)
        => Ok(await _sender.Send(new UpdateProductCommand(
            id, request.Name, request.Description, request.Brand, request.Model,
            request.ImageUrl, request.Price, request.Stock, request.Unit, request.CategoryId, request.IsActive)));

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _sender.Send(new DeleteProductCommand(id));
        return NoContent();
    }

    [HttpPatch("{id}/stock")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStock(Guid id, [FromBody] UpdateStockRequest request)
        => Ok(await _sender.Send(new UpdateStockCommand(id, request.Stock)));

    private bool CanSeePrice() =>
        User.Identity?.IsAuthenticated == true &&
        (User.IsInRole("Admin") || User.IsInRole("Seller"));
}
