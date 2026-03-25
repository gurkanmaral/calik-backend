using CalikBackend.Application.DTOs.Customers;
using MediatR;

namespace CalikBackend.Application.Features.Customers.Commands.AdjustBalance;

public record AdjustBalanceCommand(Guid Id, decimal Amount) : IRequest<CustomerResponse>;
