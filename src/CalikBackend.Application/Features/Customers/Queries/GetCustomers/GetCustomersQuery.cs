using CalikBackend.Application.DTOs.Common;
using CalikBackend.Application.DTOs.Customers;
using MediatR;

namespace CalikBackend.Application.Features.Customers.Queries.GetCustomers;

public record GetCustomersQuery(
    string? Search,
    string? City,
    string? District,
    decimal? MinBalance,
    decimal? MaxBalance,
    string? SortBy,
    bool SortDesc,
    int Page,
    int PageSize) : IRequest<PagedResult<CustomerResponse>>;
