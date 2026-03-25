using MediatR;

namespace CalikBackend.Application.Features.Invoices.Commands.DeleteInvoice;

public record DeleteInvoiceCommand(Guid Id) : IRequest;
