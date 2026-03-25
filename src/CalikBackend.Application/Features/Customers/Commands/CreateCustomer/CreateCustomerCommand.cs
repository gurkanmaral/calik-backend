using CalikBackend.Application.DTOs.Customers;
using MediatR;

namespace CalikBackend.Application.Features.Customers.Commands.CreateCustomer;

public record CreateCustomerCommand(
    string Name,
    string? PhoneNumber,
    string? CountryCode,
    string? Email,
    string? Address,
    string? City,
    string? District,
    decimal Balance) : IRequest<CustomerResponse>;
