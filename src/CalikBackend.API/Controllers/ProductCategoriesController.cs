using CalikBackend.Application.DTOs.Products;
using CalikBackend.Domain.Entities;
using CalikBackend.Infrastructure.Data;
using CalikBackend.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalikBackend.API.Controllers;

[ApiController]
[Route("api/product-categories")]
public class ProductCategoriesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProductCategoriesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] bool sortDesc = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _db.ProductCategories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c =>
                c.Name.Contains(search) ||
                (c.Description != null && c.Description.Contains(search)));

        query = sortDesc ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name);

        var result = await query
            .Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                CreatedAt = c.CreatedAt
            })
            .ToPagedResultAsync(page, pageSize);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await _db.ProductCategories.FindAsync(id);
        if (category == null)
            return NotFound(new { message = "Category not found." });

        return Ok(new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            CreatedAt = category.CreatedAt
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        var category = new ProductCategory
        {
            Name = request.Name,
            Description = request.Description,
            ImageUrl = request.ImageUrl
        };

        _db.ProductCategories.Add(category);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = category.Id }, new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            CreatedAt = category.CreatedAt
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request)
    {
        var category = await _db.ProductCategories.FindAsync(id);
        if (category == null)
            return NotFound(new { message = "Category not found." });

        category.Name = request.Name;
        category.Description = request.Description;
        category.ImageUrl = request.ImageUrl;

        await _db.SaveChangesAsync();

        return Ok(new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            CreatedAt = category.CreatedAt
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var category = await _db.ProductCategories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return NotFound(new { message = "Category not found." });

        if (category.Products.Count > 0)
            return Conflict(new { message = "Cannot delete category with existing products." });

        _db.ProductCategories.Remove(category);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
