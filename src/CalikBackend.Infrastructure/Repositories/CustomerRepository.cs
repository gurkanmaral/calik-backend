using CalikBackend.Application.DTOs.Common;
using CalikBackend.Application.DTOs.Customers;
using CalikBackend.Application.Repositories.Interfaces;
using CalikBackend.Domain.Entities;
using CalikBackend.Infrastructure.Data;
using CalikBackend.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CalikBackend.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _db;

    public CustomerRepository(AppDbContext db) => _db = db;

    public async Task<PagedResult<CustomerResponse>> GetPagedAsync(
        string? search, string? city, string? district,
        decimal? minBalance, decimal? maxBalance,
        string? sortBy, bool sortDesc, int page, int pageSize,
        CancellationToken ct)
    {
        var query = _db.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c =>
                c.Name.Contains(search) ||
                (c.Email != null && c.Email.Contains(search)) ||
                (c.PhoneNumber != null && c.PhoneNumber.Contains(search)));

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(c => c.City == city);

        if (!string.IsNullOrWhiteSpace(district))
            query = query.Where(c => c.District == district);

        if (minBalance.HasValue)
            query = query.Where(c => c.Balance >= minBalance.Value);

        if (maxBalance.HasValue)
            query = query.Where(c => c.Balance <= maxBalance.Value);

        query = sortBy?.ToLower() switch
        {
            "balance"   => sortDesc ? query.OrderByDescending(c => c.Balance)   : query.OrderBy(c => c.Balance),
            "createdat" => sortDesc ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
            _           => sortDesc ? query.OrderByDescending(c => c.Name)      : query.OrderBy(c => c.Name),
        };

        return await query
            .Select(c => new CustomerResponse
            {
                Id = c.Id,
                Name = c.Name,
                PhoneNumber = c.PhoneNumber,
                CountryCode = c.CountryCode,
                Email = c.Email,
                Address = c.Address,
                City = c.City,
                District = c.District,
                Balance = c.Balance,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToPagedResultAsync(page, pageSize);
    }

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct)
        => _db.Customers.FindAsync([id], ct).AsTask();

    public async Task AddAsync(Customer customer, CancellationToken ct)
        => await _db.Customers.AddAsync(customer, ct);

    public void Remove(Customer customer)
        => _db.Customers.Remove(customer);

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}
