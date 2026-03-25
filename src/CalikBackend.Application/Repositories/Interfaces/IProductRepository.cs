using CalikBackend.Application.DTOs.Common;
using CalikBackend.Application.DTOs.Products;
using CalikBackend.Domain.Entities;

namespace CalikBackend.Application.Repositories.Interfaces;

public interface IProductRepository
{
    Task<PagedResult<ProductResponse>> GetPagedAsync(
        Guid? categoryId, string? search, string? brand,
        decimal? minPrice, decimal? maxPrice, bool? inStock,
        string? sortBy, bool sortDesc, int page, int pageSize,
        bool includePrice, CancellationToken ct);
    Task<Product?> GetByIdAsync(Guid id, bool includeCategory, CancellationToken ct);
    Task<List<Product>> GetByIdsAsync(List<Guid> ids, bool activeOnly, CancellationToken ct);
    Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken ct);
    Task AddAsync(Product product, CancellationToken ct);
    void Remove(Product product);
    Task SaveChangesAsync(CancellationToken ct);
}
