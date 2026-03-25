using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Application.DTOs.Invoices;
using CalikBackend.Application.Repositories.Interfaces;
using CalikBackend.Domain.Entities;
using MediatR;

namespace CalikBackend.Application.Features.Invoices.Commands.UpdateInvoice;

public class UpdateInvoiceCommandHandler : IRequestHandler<UpdateInvoiceCommand, InvoiceResponse>
{
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly IProductRepository _productRepo;

    public UpdateInvoiceCommandHandler(IInvoiceRepository invoiceRepo, IProductRepository productRepo)
    {
        _invoiceRepo = invoiceRepo;
        _productRepo = productRepo;
    }

    public async Task<InvoiceResponse> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepo.GetByIdAsync(request.Id, includeItems: true, cancellationToken)
            ?? throw new NotFoundException("Invoice not found.");

        if (invoice.Status != InvoiceStatus.Draft)
            throw new BadRequestException("Only Draft invoices can be updated.");

        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _productRepo.GetByIdsAsync(productIds, activeOnly: true, cancellationToken);

        if (products.Count != productIds.Count)
            throw new BadRequestException("One or more products not found or inactive.");

        var productMap = products.ToDictionary(p => p.Id);

        // Calculate restored stock for stock-availability check
        var oldStockByProduct = invoice.Items
            .GroupBy(i => i.ProductId)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.Quantity));

        foreach (var itemReq in request.Items)
        {
            var product = productMap[itemReq.ProductId];
            var restoredQty = oldStockByProduct.GetValueOrDefault(product.Id, 0);
            var availableStock = product.Stock + restoredQty;
            if (availableStock < itemReq.Quantity)
                throw new BadRequestException($"Insufficient stock for '{product.Name}'. Available: {availableStock}.");
        }

        // Restore stock for old items
        var oldProductIds = invoice.Items.Select(i => i.ProductId).Distinct().ToList();
        var oldProducts = await _productRepo.GetByIdsAsync(oldProductIds, activeOnly: false, cancellationToken);
        var oldProductMap = oldProducts.ToDictionary(p => p.Id);
        foreach (var oldItem in invoice.Items)
        {
            oldProductMap[oldItem.ProductId].Stock += oldItem.Quantity;
            oldProductMap[oldItem.ProductId].UpdatedAt = DateTime.UtcNow;
        }

        invoice.InvoiceDate = request.InvoiceDate ?? invoice.InvoiceDate;
        invoice.CustomerName = request.CustomerName;
        invoice.CustomerAddress = request.CustomerAddress;
        invoice.CustomerTaxNumber = request.CustomerTaxNumber;
        invoice.CustomerEmail = request.CustomerEmail;
        invoice.CustomerPhone = request.CustomerPhone;
        invoice.Notes = request.Notes;
        invoice.UpdatedAt = DateTime.UtcNow;

        _invoiceRepo.RemoveItems(invoice.Items.ToList());
        invoice.Items.Clear();

        foreach (var itemReq in request.Items)
        {
            var product = productMap[itemReq.ProductId];
            invoice.Items.Add(new InvoiceItem
            {
                InvoiceId = invoice.Id,
                ProductId = product.Id,
                ProductName = product.Name,
                Unit = product.Unit,
                UnitPrice = product.Price,
                Quantity = itemReq.Quantity,
                TotalPrice = product.Price * itemReq.Quantity
            });
            product.Stock -= itemReq.Quantity;
            product.UpdatedAt = DateTime.UtcNow;
        }

        invoice.TotalAmount = invoice.Items.Sum(i => i.TotalPrice);

        await _invoiceRepo.SaveChangesAsync(cancellationToken);

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
