using MediatR;

namespace CalikBackend.Application.Features.Customers.Commands.DeleteCustomer;

public record DeleteCustomerCommand(Guid Id) : IRequest;
