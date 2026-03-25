using CalikBackend.Application.DTOs.Common;
using CalikBackend.Application.DTOs.Invoices;
using MediatR;

namespace CalikBackend.Application.Features.Invoices.Queries.GetInvoices;

public record GetInvoicesQuery(
    string? Search,
    string? Status,
    DateTime? DateFrom,
    DateTime? DateTo,
    decimal? MinAmount,
    decimal? MaxAmount,
    string? SortBy,
    bool SortDesc,
    int Page,
    int PageSize) : IRequest<PagedResult<InvoiceResponse>>;
