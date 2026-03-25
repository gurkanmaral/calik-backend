using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Application.DTOs.Customers;
using CalikBackend.Application.Repositories.Interfaces;
using MediatR;

namespace CalikBackend.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, CustomerResponse>
{
    private readonly ICustomerRepository _repo;

    public UpdateCustomerCommandHandler(ICustomerRepository repo) => _repo = repo;

    public async Task<CustomerResponse> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Customer not found.");

        customer.Name = request.Name;
        customer.PhoneNumber = request.PhoneNumber;
        customer.CountryCode = request.CountryCode;
        customer.Email = request.Email;
        customer.Address = request.Address;
        customer.City = request.City;
        customer.District = request.District;
        customer.Balance = request.Balance;
        customer.UpdatedAt = DateTime.UtcNow;

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
