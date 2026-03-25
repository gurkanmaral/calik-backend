using CalikBackend.Application.DTOs.Invoices;
using MediatR;

namespace CalikBackend.Application.Features.Invoices.Commands.UpdateInvoice;

public record UpdateInvoiceCommand(
    Guid Id,
    DateTime? InvoiceDate,
    string CustomerName,
    string? CustomerAddress,
    string? CustomerTaxNumber,
    string? CustomerEmail,
    string? CustomerPhone,
    string? Notes,
    List<InvoiceItemRequest> Items) : IRequest<InvoiceResponse>;
