using CalikBackend.Application.DTOs.Common;
using CalikBackend.Application.DTOs.Customers;
using CalikBackend.Domain.Entities;

namespace CalikBackend.Application.Repositories.Interfaces;

public interface ICustomerRepository
{
    Task<PagedResult<CustomerResponse>> GetPagedAsync(
        string? search, string? city, string? district,
        decimal? minBalance, decimal? maxBalance,
        string? sortBy, bool sortDesc, int page, int pageSize,
        CancellationToken ct);
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Customer customer, CancellationToken ct);
    void Remove(Customer customer);
    Task SaveChangesAsync(CancellationToken ct);
}
