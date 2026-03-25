using CalikBackend.Application.DTOs.Invoices;
using CalikBackend.Domain.Entities;
using MediatR;

namespace CalikBackend.Application.Features.Invoices.Commands.UpdateInvoiceStatus;

public record UpdateInvoiceStatusCommand(Guid Id, InvoiceStatus Status) : IRequest<InvoiceResponse>;
