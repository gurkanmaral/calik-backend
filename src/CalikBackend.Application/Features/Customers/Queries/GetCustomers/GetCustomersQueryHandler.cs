using CalikBackend.Application.DTOs.Common;
using CalikBackend.Application.DTOs.Customers;
using CalikBackend.Application.Repositories.Interfaces;
using MediatR;

namespace CalikBackend.Application.Features.Customers.Queries.GetCustomers;

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, PagedResult<CustomerResponse>>
{
    private readonly ICustomerRepository _repo;

    public GetCustomersQueryHandler(ICustomerRepository repo) => _repo = repo;

    public Task<PagedResult<CustomerResponse>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        => _repo.GetPagedAsync(
            request.Search, request.City, request.District,
            request.MinBalance, request.MaxBalance,
            request.SortBy, request.SortDesc, request.Page, request.PageSize,
            cancellationToken);
}
