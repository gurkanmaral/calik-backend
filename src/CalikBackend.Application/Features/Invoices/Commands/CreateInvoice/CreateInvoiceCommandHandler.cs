using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Application.DTOs.Invoices;
using CalikBackend.Application.Repositories.Interfaces;
using CalikBackend.Domain.Entities;
using MediatR;

namespace CalikBackend.Application.Features.Invoices.Commands.CreateInvoice;

public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, InvoiceResponse>
{
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly IProductRepository _productRepo;

    public CreateInvoiceCommandHandler(IInvoiceRepository invoiceRepo, IProductRepository productRepo)
    {
        _invoiceRepo = invoiceRepo;
        _productRepo = productRepo;
    }

    public async Task<InvoiceResponse> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _productRepo.GetByIdsAsync(productIds, activeOnly: true, cancellationToken);

        if (products.Count != productIds.Count)
            throw new BadRequestException("One or more products not found or inactive.");

        var productMap = products.ToDictionary(p => p.Id);

        foreach (var itemReq in request.Items)
        {
            var product = productMap[itemReq.ProductId];
            if (product.Stock < itemReq.Quantity)
                throw new BadRequestException($"Insufficient stock for '{product.Name}'. Available: {product.Stock}.");
        }

        var invoice = new Invoice
        {
            InvoiceDate = request.InvoiceDate ?? DateTime.UtcNow,
            CustomerName = request.CustomerName,
            CustomerAddress = request.CustomerAddress,
            CustomerTaxNumber = request.CustomerTaxNumber,
            CustomerEmail = request.CustomerEmail,
            CustomerPhone = request.CustomerPhone,
            Notes = request.Notes
        };

        var year = DateTime.UtcNow.Year;
        var count = await _invoiceRepo.CountForYearAsync(year, cancellationToken) + 1;
        invoice.InvoiceNumber = $"INV-{year}-{count:D4}";

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

        await _invoiceRepo.AddAsync(invoice, cancellationToken);
        // Both product stock changes and the new invoice are committed in one SaveChanges
        // (shared AppDbContext via scoped DI)
        await _invoiceRepo.SaveChangesAsync(cancellationToken);

        return MapToResponse(invoice);
    }

    private static InvoiceResponse MapToResponse(Invoice i) => new()
    {
        Id = i.Id,
        InvoiceNumber = i.InvoiceNumber,
        InvoiceDate = i.InvoiceDate,
        Status = i.Status,
        CustomerName = i.CustomerName,
        CustomerAddress = i.CustomerAddress,
        CustomerTaxNumber = i.CustomerTaxNumber,
        CustomerEmail = i.CustomerEmail,
        CustomerPhone = i.CustomerPhone,
        Notes = i.Notes,
        TotalAmount = i.TotalAmount,
        Items = i.Items.Select(item => new InvoiceItemResponse
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            Unit = item.Unit,
            UnitPrice = item.UnitPrice,
            Quantity = item.Quantity,
            TotalPrice = item.TotalPrice
        }).ToList(),
        CreatedAt = i.CreatedAt,
        UpdatedAt = i.UpdatedAt
    };
}
