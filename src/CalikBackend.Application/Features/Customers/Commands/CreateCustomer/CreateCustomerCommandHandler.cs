using CalikBackend.Application.DTOs.Customers;
using CalikBackend.Application.Repositories.Interfaces;
using CalikBackend.Domain.Entities;
using MediatR;

namespace CalikBackend.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerResponse>
{
    private readonly ICustomerRepository _repo;

    public CreateCustomerCommandHandler(ICustomerRepository repo) => _repo = repo;

    public async Task<CustomerResponse> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            CountryCode = request.CountryCode,
            Email = request.Email,
            Address = request.Address,
            City = request.City,
            District = request.District,
            Balance = request.Balance
        };

        await _repo.AddAsync(customer, cancellationToken);
        await _repo.SaveChangesAsync(cancellationToken);

        return new CustomerResponse
        {
            Id = customer.Id,
            Name = customer.Name,
            PhoneNumber = customer.PhoneNumber,
            CountryCode = customer.CountryCode,
            Email = customer.Email,
            Address = customer.Address,
            City = customer.City,
            District = customer.District,
            Balance = customer.Balance,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }
}
