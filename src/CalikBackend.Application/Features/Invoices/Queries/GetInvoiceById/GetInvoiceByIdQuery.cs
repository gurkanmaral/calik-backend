using CalikBackend.Application.DTOs.Invoices;
using MediatR;

namespace CalikBackend.Application.Features.Invoices.Queries.GetInvoiceById;

public record GetInvoiceByIdQuery(Guid Id) : IRequest<InvoiceResponse>;
