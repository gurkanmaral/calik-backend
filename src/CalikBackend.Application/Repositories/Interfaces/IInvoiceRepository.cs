using CalikBackend.Application.DTOs.Common;
using CalikBackend.Application.DTOs.Invoices;
using CalikBackend.Domain.Entities;

namespace CalikBackend.Application.Repositories.Interfaces;


public interface IInvoiceRepository
{
    Task<PagedResult<InvoiceResponse>> GetPagedAsync(
        string? search, string? status, DateTime? dateFrom, DateTime? dateTo,
        decimal? minAmount, decimal? maxAmount,
        string? sortBy, bool sortDesc, int page, int pageSize,
        CancellationToken ct);
    Task<Invoice?> GetByIdAsync(Guid id, bool includeItems, CancellationToken ct);
    Task<int> CountForYearAsync(int year, CancellationToken ct);
    Task AddAsync(Invoice invoice, CancellationToken ct);
    void Remove(Invoice invoice);
    void RemoveItems(IEnumerable<InvoiceItem> items);
    Task SaveChangesAsync(CancellationToken ct);
}
