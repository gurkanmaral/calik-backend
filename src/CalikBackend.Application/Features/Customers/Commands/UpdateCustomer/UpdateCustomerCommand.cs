using CalikBackend.Application.DTOs.Customers;
using MediatR;

namespace CalikBackend.Application.Features.Customers.Commands.UpdateCustomer;

public record UpdateCustomerCommand(
    Guid Id,
    string Name,
    string? PhoneNumber,
    string? CountryCode,
    string? Email,
    string? Address,
    string? City,
    string? District,
    decimal Balance) : IRequest<CustomerResponse>;
