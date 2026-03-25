using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Application.DTOs.Customers;
using CalikBackend.Application.Repositories.Interfaces;
using MediatR;

namespace CalikBackend.Application.Features.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerResponse>
{
    private readonly ICustomerRepository _repo;

    public GetCustomerByIdQueryHandler(ICustomerRepository repo) => _repo = repo;

    public async Task<CustomerResponse> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Customer not found.");

        return MapToResponse(customer);
    }

    private static CustomerResponse MapToResponse(CalikBackend.Domain.Entities.Customer c) => new()
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
    };
}
