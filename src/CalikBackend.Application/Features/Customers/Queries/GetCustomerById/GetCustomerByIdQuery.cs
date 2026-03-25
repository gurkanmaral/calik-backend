using CalikBackend.Application.DTOs.Customers;
using MediatR;

namespace CalikBackend.Application.Features.Customers.Queries.GetCustomerById;

public record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerResponse>;
