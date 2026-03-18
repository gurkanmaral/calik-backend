using CalikBackend.Application.DTOs.Common;
using CalikBackend.Application.DTOs.Products;
using CalikBackend.Domain.Entities;
using CalikBackend.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalikBackend.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db)
    {
        _db = db;
    }

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
    {
        var includePrice = CanSeePrice();

        var query = _db.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive);

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p =>
                p.Name.Contains(search) ||
                (p.Brand != null && p.Brand.Contains(search)) ||
                (p.Model != null && p.Model.Contains(search)));

        if (!string.IsNullOrWhiteSpace(brand))
            query = query.Where(p => p.Brand == brand);

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        if (inStock == true)
            query = query.Where(p => p.Stock > 0);

        query = sortBy?.ToLower() switch
        {
            "price"     => sortDesc ? query.OrderByDescending(p => p.Price)     : query.OrderBy(p => p.Price),
            "stock"     => sortDesc ? query.OrderByDescending(p => p.Stock)     : query.OrderBy(p => p.Stock),
            "createdat" => sortDesc ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            _           => sortDesc ? query.OrderByDescending(p => p.Name)      : query.OrderBy(p => p.Name),
        };

        var pageNum = Math.Max(1, page);
        var pageSz = Math.Clamp(pageSize, 1, 100);
        var totalCount = await query.CountAsync();
        var products = await query.Skip((pageNum - 1) * pageSz).Take(pageSz).ToListAsync();

        return Ok(new PagedResult<ProductResponse>
        {
            Items = products.Select(p => MapToResponse(p, includePrice)).ToList(),
            TotalCount = totalCount,
            Page = pageNum,
            PageSize = pageSz
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _db.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound(new { message = "Product not found." });

        return Ok(MapToResponse(product, CanSeePrice()));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        var categoryExists = await _db.ProductCategories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
            return BadRequest(new { message = "Category not found." });

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Brand = request.Brand,
            Model = request.Model,
            ImageUrl = request.ImageUrl,
            Price = request.Price,
            Stock = request.Stock,
            Unit = request.Unit,
            CategoryId = request.CategoryId
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        await _db.Entry(product).Reference(p => p.Category).LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, MapToResponse(product, true));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request)
    {
        var product = await _db.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound(new { message = "Product not found." });

        var categoryExists = await _db.ProductCategories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
            return BadRequest(new { message = "Category not found." });

        product.Name = request.Name;
        product.Description = request.Description;
        product.Brand = request.Brand;
        product.Model = request.Model;
        product.ImageUrl = request.ImageUrl;
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.Unit = request.Unit;
        product.CategoryId = request.CategoryId;
        product.IsActive = request.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        await _db.Entry(product).Reference(p => p.Category).LoadAsync();

        return Ok(MapToResponse(product, true));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
            return NotFound(new { message = "Product not found." });

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}/stock")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStock(Guid id, [FromBody] UpdateStockRequest request)
    {
        var product = await _db.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound(new { message = "Product not found." });

        product.Stock = request.Stock;
        product.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(MapToResponse(product, true));
    }

    private bool CanSeePrice() =>
        User.Identity?.IsAuthenticated == true &&
        (User.IsInRole("Admin") || User.IsInRole("Seller"));

    private static ProductResponse MapToResponse(Product p, bool includePrice) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Brand = p.Brand,
        Model = p.Model,
        ImageUrl = p.ImageUrl,
        Price = includePrice ? p.Price : null,
        Stock = p.Stock,
        Unit = p.Unit,
        IsActive = p.IsActive,
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.Name ?? string.Empty,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt
    };
}
