using CalikBackend.Application.DTOs.Common;
using CalikBackend.Application.DTOs.Invoices;
using CalikBackend.Application.Repositories.Interfaces;
using CalikBackend.Domain.Entities;
using CalikBackend.Infrastructure.Data;
using CalikBackend.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CalikBackend.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly AppDbContext _db;

    public InvoiceRepository(AppDbContext db) => _db = db;

    public async Task<PagedResult<InvoiceResponse>> GetPagedAsync(
        string? search, string? status, DateTime? dateFrom, DateTime? dateTo,
        decimal? minAmount, decimal? maxAmount,
        string? sortBy, bool sortDesc, int page, int pageSize,
        CancellationToken ct)
    {
        var query = _db.Invoices.Include(i => i.Items).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(i =>
                i.InvoiceNumber.Contains(search) ||
                (i.CustomerName != null && i.CustomerName.Contains(search)) ||
                (i.CustomerEmail != null && i.CustomerEmail.Contains(search)));

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<InvoiceStatus>(status, ignoreCase: true, out var parsedStatus))
            query = query.Where(i => i.Status == parsedStatus);

        if (dateFrom.HasValue)
            query = query.Where(i => i.InvoiceDate >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(i => i.InvoiceDate <= dateTo.Value);

        if (minAmount.HasValue)
            query = query.Where(i => i.TotalAmount >= minAmount.Value);

        if (maxAmount.HasValue)
            query = query.Where(i => i.TotalAmount <= maxAmount.Value);

        query = sortBy?.ToLower() switch
        {
            "invoicenumber" => sortDesc ? query.OrderByDescending(i => i.InvoiceNumber) : query.OrderBy(i => i.InvoiceNumber),
            "customer"      => sortDesc ? query.OrderByDescending(i => i.CustomerName)  : query.OrderBy(i => i.CustomerName),
            "amount"        => sortDesc ? query.OrderByDescending(i => i.TotalAmount)   : query.OrderBy(i => i.TotalAmount),
            "createdat"     => sortDesc ? query.OrderByDescending(i => i.CreatedAt)     : query.OrderBy(i => i.CreatedAt),
            _               => sortDesc ? query.OrderByDescending(i => i.InvoiceDate)   : query.OrderBy(i => i.InvoiceDate),
        };

        return await query.Select(i => MapToResponse(i)).ToPagedResultAsync(page, pageSize);
    }

    public Task<Invoice?> GetByIdAsync(Guid id, bool includeItems, CancellationToken ct)
    {
        var query = _db.Invoices.AsQueryable();
        if (includeItems)
            query = query.Include(i => i.Items);
        return query.FirstOrDefaultAsync(i => i.Id == id, ct);
    }

    public Task<int> CountForYearAsync(int year, CancellationToken ct)
        => _db.Invoices.CountAsync(i => i.InvoiceDate.Year == year, ct);

    public async Task AddAsync(Invoice invoice, CancellationToken ct)
        => await _db.Invoices.AddAsync(invoice, ct);

    public void Remove(Invoice invoice)
        => _db.Invoices.Remove(invoice);

    public void RemoveItems(IEnumerable<InvoiceItem> items)
        => _db.InvoiceItems.RemoveRange(items);

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);

    internal static InvoiceResponse MapToResponse(Invoice i) => new()
    {
        Id = i.Id,
        InvoiceNumber = i.InvoiceNumber,
        InvoiceDate = i.InvoiceDate,
        Status = i.Status,
        CustomerName = i.CustomerName,
        CustomerAddress = i.CustomerAddress,
        CustomerTaxNumber = i.CustomerTaxNumber,
        CustomerEmail = i.CustomerEmail,
        CustomerPhone = i.CustomerPhone,
        Notes = i.Notes,
        TotalAmount = i.TotalAmount,
        Items = i.Items.Select(item => new InvoiceItemResponse
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            Unit = item.Unit,
            UnitPrice = item.UnitPrice,
            Quantity = item.Quantity,
            TotalPrice = item.TotalPrice
        }).ToList(),
        CreatedAt = i.CreatedAt,
        UpdatedAt = i.UpdatedAt
    };
}
