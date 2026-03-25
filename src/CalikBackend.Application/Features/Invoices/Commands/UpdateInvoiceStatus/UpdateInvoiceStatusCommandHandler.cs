using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Application.DTOs.Invoices;
using CalikBackend.Application.Repositories.Interfaces;
using MediatR;

namespace CalikBackend.Application.Features.Invoices.Commands.UpdateInvoiceStatus;

public class UpdateInvoiceStatusCommandHandler : IRequestHandler<UpdateInvoiceStatusCommand, InvoiceResponse>
{
    private readonly IInvoiceRepository _repo;

    public UpdateInvoiceStatusCommandHandler(IInvoiceRepository repo) => _repo = repo;

    public async Task<InvoiceResponse> Handle(UpdateInvoiceStatusCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _repo.GetByIdAsync(request.Id, includeItems: true, cancellationToken)
            ?? throw new NotFoundException("Invoice not found.");

        invoice.Status = request.Status;
        invoice.UpdatedAt = DateTime.UtcNow;

        await _repo.SaveChangesAsync(cancellationToken);

        return new InvoiceResponse
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            InvoiceDate = invoice.InvoiceDate,
            Status = invoice.Status,
            CustomerName = invoice.CustomerName,
            CustomerAddress = invoice.CustomerAddress,
            CustomerTaxNumber = invoice.CustomerTaxNumber,
            CustomerEmail = invoice.CustomerEmail,
            CustomerPhone = invoice.CustomerPhone,
            Notes = invoice.Notes,
            TotalAmount = invoice.TotalAmount,
            Items = invoice.Items.Select(item => new InvoiceItemResponse
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Unit = item.Unit,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                TotalPrice = item.TotalPrice
            }).ToList(),
            CreatedAt = invoice.CreatedAt,
            UpdatedAt = invoice.UpdatedAt
        };
    }
}
