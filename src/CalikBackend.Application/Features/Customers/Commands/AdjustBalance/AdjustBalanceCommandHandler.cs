using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Application.DTOs.Customers;
using CalikBackend.Application.Repositories.Interfaces;
using MediatR;

namespace CalikBackend.Application.Features.Customers.Commands.AdjustBalance;

public class AdjustBalanceCommandHandler : IRequestHandler<AdjustBalanceCommand, CustomerResponse>
{
    private readonly ICustomerRepository _repo;

    public AdjustBalanceCommandHandler(ICustomerRepository repo) => _repo = repo;

    public async Task<CustomerResponse> Handle(AdjustBalanceCommand request, CancellationToken cancellationToken)
    {
        var customer = await _repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Customer not found.");

        customer.Balance += request.Amount;
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
