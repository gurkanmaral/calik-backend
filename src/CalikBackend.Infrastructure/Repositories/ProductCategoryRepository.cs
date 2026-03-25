using CalikBackend.Application.DTOs.Common;
using CalikBackend.Application.DTOs.Products;
using CalikBackend.Application.Repositories.Interfaces;
using CalikBackend.Domain.Entities;
using CalikBackend.Infrastructure.Data;
using CalikBackend.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CalikBackend.Infrastructure.Repositories;

public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly AppDbContext _db;

    public ProductCategoryRepository(AppDbContext db) => _db = db;

    public async Task<PagedResult<CategoryResponse>> GetPagedAsync(
        string? search, bool sortDesc, int page, int pageSize, CancellationToken ct)
    {
        var query = _db.ProductCategories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c =>
                c.Name.Contains(search) ||
                (c.Description != null && c.Description.Contains(search)));

        query = sortDesc ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name);

        return await query
            .Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                CreatedAt = c.CreatedAt
            })
            .ToPagedResultAsync(page, pageSize);
    }

    public Task<ProductCategory?> GetByIdAsync(Guid id, CancellationToken ct)
        => _db.ProductCategories.FindAsync([id], ct).AsTask();

    public Task<ProductCategory?> GetByIdWithProductsAsync(Guid id, CancellationToken ct)
        => _db.ProductCategories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task AddAsync(ProductCategory category, CancellationToken ct)
        => await _db.ProductCategories.AddAsync(category, ct);

    public void Remove(ProductCategory category)
        => _db.ProductCategories.Remove(category);

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}
