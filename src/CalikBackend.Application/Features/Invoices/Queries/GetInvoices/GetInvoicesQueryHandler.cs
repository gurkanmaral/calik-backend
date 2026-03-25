using CalikBackend.Application.DTOs.Common;
using CalikBackend.Application.DTOs.Invoices;
using CalikBackend.Application.Repositories.Interfaces;
using MediatR;

namespace CalikBackend.Application.Features.Invoices.Queries.GetInvoices;

public class GetInvoicesQueryHandler : IRequestHandler<GetInvoicesQuery, PagedResult<InvoiceResponse>>
{
    private readonly IInvoiceRepository _repo;

    public GetInvoicesQueryHandler(IInvoiceRepository repo) => _repo = repo;

    public Task<PagedResult<InvoiceResponse>> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
        => _repo.GetPagedAsync(
            request.Search, request.Status, request.DateFrom, request.DateTo,
            request.MinAmount, request.MaxAmount,
            request.SortBy, request.SortDesc, request.Page, request.PageSize,
            cancellationToken);
}
