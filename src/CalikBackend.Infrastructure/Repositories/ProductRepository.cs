using CalikBackend.Application.DTOs.Common;
using CalikBackend.Application.DTOs.Products;
using CalikBackend.Application.Repositories.Interfaces;
using CalikBackend.Domain.Entities;
using CalikBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CalikBackend.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;

    public ProductRepository(AppDbContext db) => _db = db;

    public async Task<PagedResult<ProductResponse>> GetPagedAsync(
        Guid? categoryId, string? search, string? brand,
        decimal? minPrice, decimal? maxPrice, bool? inStock,
        string? sortBy, bool sortDesc, int page, int pageSize,
        bool includePrice, CancellationToken ct)
    {
        var query = _db.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .AsQueryable();

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

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var totalCount = await query.CountAsync(ct);
        var products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return new PagedResult<ProductResponse>
        {
            Items = products.Select(p => MapToResponse(p, includePrice)).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public Task<Product?> GetByIdAsync(Guid id, bool includeCategory, CancellationToken ct)
    {
        var query = _db.Products.AsQueryable();
        if (includeCategory)
            query = query.Include(p => p.Category);
        return query.FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public Task<List<Product>> GetByIdsAsync(List<Guid> ids, bool activeOnly, CancellationToken ct)
    {
        var query = _db.Products.AsQueryable();
        if (activeOnly)
            query = query.Where(p => p.IsActive);
        return query.Where(p => ids.Contains(p.Id)).ToListAsync(ct);
    }

    public Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken ct)
        => _db.ProductCategories.AnyAsync(c => c.Id == categoryId, ct);

    public async Task AddAsync(Product product, CancellationToken ct)
        => await _db.Products.AddAsync(product, ct);

    public void Remove(Product product)
        => _db.Products.Remove(product);

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);

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
