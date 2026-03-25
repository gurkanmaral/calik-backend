using CalikBackend.Application.DTOs.Common;
using CalikBackend.Application.DTOs.Products;
using CalikBackend.Domain.Entities;

namespace CalikBackend.Application.Repositories.Interfaces;

public interface IProductCategoryRepository
{
    Task<PagedResult<CategoryResponse>> GetPagedAsync(string? search, bool sortDesc, int page, int pageSize, CancellationToken ct);
    Task<ProductCategory?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<ProductCategory?> GetByIdWithProductsAsync(Guid id, CancellationToken ct);
    Task AddAsync(ProductCategory category, CancellationToken ct);
    void Remove(ProductCategory category);
    Task SaveChangesAsync(CancellationToken ct);
}
