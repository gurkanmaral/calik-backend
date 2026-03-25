using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Application.Repositories.Interfaces;
using CalikBackend.Domain.Entities;
using MediatR;

namespace CalikBackend.Application.Features.Invoices.Commands.DeleteInvoice;

public class DeleteInvoiceCommandHandler : IRequestHandler<DeleteInvoiceCommand>
{
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly IProductRepository _productRepo;

    public DeleteInvoiceCommandHandler(IInvoiceRepository invoiceRepo, IProductRepository productRepo)
    {
        _invoiceRepo = invoiceRepo;
        _productRepo = productRepo;
    }

    public async Task Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepo.GetByIdAsync(request.Id, includeItems: true, cancellationToken)
            ?? throw new NotFoundException("Invoice not found.");

        if (invoice.Status != InvoiceStatus.Draft)
            throw new BadRequestException("Only Draft invoices can be deleted.");

        // Restore stock
        var productIds = invoice.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _productRepo.GetByIdsAsync(productIds, activeOnly: false, cancellationToken);
        var productMap = products.ToDictionary(p => p.Id);

        foreach (var item in invoice.Items)
        {
            if (productMap.TryGetValue(item.ProductId, out var product))
            {
                product.Stock += item.Quantity;
                product.UpdatedAt = DateTime.UtcNow;
            }
        }

        _invoiceRepo.Remove(invoice);
        await _invoiceRepo.SaveChangesAsync(cancellationToken);
    }
}
