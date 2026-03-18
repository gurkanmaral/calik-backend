using CalikBackend.Application.DTOs.Invoices;
using CalikBackend.Domain.Entities;
using CalikBackend.Infrastructure.Data;
using CalikBackend.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalikBackend.API.Controllers;

[ApiController]
[Route("api/invoices")]
[Authorize(Roles = "Admin")]
public class InvoicesController : ControllerBase
{
    private readonly AppDbContext _db;

    public InvoicesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] decimal? minAmount,
        [FromQuery] decimal? maxAmount,
        [FromQuery] string? sortBy,
        [FromQuery] bool sortDesc = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _db.Invoices.Include(i => i.Items).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(i =>
                i.InvoiceNumber.Contains(search) ||
                (i.CustomerName != null && i.CustomerName.Contains(search)) ||
                (i.CustomerEmail != null && i.CustomerEmail.Contains(search)));

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<InvoiceStatus>(status, ignoreCase: true, out var parsedStatus))
            query = query.Where(i => i.Status == parsedStatus);

        if (dateFrom.HasValue)
            query = query.Where(i => i.InvoiceDate >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(i => i.InvoiceDate <= dateTo.Value);

        if (minAmount.HasValue)
            query = query.Where(i => i.TotalAmount >= minAmount.Value);

        if (maxAmount.HasValue)
            query = query.Where(i => i.TotalAmount <= maxAmount.Value);

        query = sortBy?.ToLower() switch
        {
            "invoicenumber" => sortDesc ? query.OrderByDescending(i => i.InvoiceNumber)  : query.OrderBy(i => i.InvoiceNumber),
            "customer"      => sortDesc ? query.OrderByDescending(i => i.CustomerName)   : query.OrderBy(i => i.CustomerName),
            "amount"        => sortDesc ? query.OrderByDescending(i => i.TotalAmount)    : query.OrderBy(i => i.TotalAmount),
            "createdat"     => sortDesc ? query.OrderByDescending(i => i.CreatedAt)      : query.OrderBy(i => i.CreatedAt),
            _               => sortDesc ? query.OrderByDescending(i => i.InvoiceDate)    : query.OrderBy(i => i.InvoiceDate),
        };

        var result = await query
            .Select(i => MapToResponse(i))
            .ToPagedResultAsync(page, pageSize);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var invoice = await _db.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null)
            return NotFound(new { message = "Invoice not found." });

        return Ok(MapToResponse(invoice));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceRequest request)
    {
        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _db.Products
            .Where(p => productIds.Contains(p.Id) && p.IsActive)
            .ToListAsync();

        if (products.Count != productIds.Count)
            return BadRequest(new { message = "One or more products not found or inactive." });

        var productMap = products.ToDictionary(p => p.Id);

        // Check sufficient stock
        foreach (var itemReq in request.Items)
        {
            var product = productMap[itemReq.ProductId];
            if (product.Stock < itemReq.Quantity)
                return BadRequest(new { message = $"Insufficient stock for '{product.Name}'. Available: {product.Stock}." });
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
        var count = await _db.Invoices.CountAsync(i => i.InvoiceDate.Year == year) + 1;
        invoice.InvoiceNumber = $"INV-{year}-{count:D4}";

        foreach (var itemReq in request.Items)
        {
            var product = productMap[itemReq.ProductId];
            var item = new InvoiceItem
            {
                InvoiceId = invoice.Id,
                ProductId = product.Id,
                ProductName = product.Name,
                Unit = product.Unit,
                UnitPrice = product.Price,
                Quantity = itemReq.Quantity,
                TotalPrice = product.Price * itemReq.Quantity
            };
            invoice.Items.Add(item);
            product.Stock -= itemReq.Quantity;
            product.UpdatedAt = DateTime.UtcNow;
        }

        invoice.TotalAmount = invoice.Items.Sum(i => i.TotalPrice);

        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, MapToResponse(invoice));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateInvoiceRequest request)
    {
        var invoice = await _db.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null)
            return NotFound(new { message = "Invoice not found." });

        if (invoice.Status != InvoiceStatus.Draft)
            return BadRequest(new { message = "Only Draft invoices can be updated." });

        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _db.Products
            .Where(p => productIds.Contains(p.Id) && p.IsActive)
            .ToListAsync();

        if (products.Count != productIds.Count)
            return BadRequest(new { message = "One or more products not found or inactive." });

        var productMap = products.ToDictionary(p => p.Id);

        // Check sufficient stock for new items (accounting for what will be restored)
        var oldStockByProduct = invoice.Items
            .GroupBy(i => i.ProductId)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.Quantity));

        foreach (var itemReq in request.Items)
        {
            var product = productMap[itemReq.ProductId];
            var restoredQty = oldStockByProduct.GetValueOrDefault(product.Id, 0);
            var availableStock = product.Stock + restoredQty;
            if (availableStock < itemReq.Quantity)
                return BadRequest(new { message = $"Insufficient stock for '{product.Name}'. Available: {availableStock}." });
        }

        // Restore stock for old items
        var oldProductIds = invoice.Items.Select(i => i.ProductId).Distinct().ToList();
        var oldProducts = await _db.Products.Where(p => oldProductIds.Contains(p.Id)).ToListAsync();
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

        _db.InvoiceItems.RemoveRange(invoice.Items);
        invoice.Items.Clear();

        foreach (var itemReq in request.Items)
        {
            var product = productMap[itemReq.ProductId];
            var item = new InvoiceItem
            {
                InvoiceId = invoice.Id,
                ProductId = product.Id,
                ProductName = product.Name,
                Unit = product.Unit,
                UnitPrice = product.Price,
                Quantity = itemReq.Quantity,
                TotalPrice = product.Price * itemReq.Quantity
            };
            invoice.Items.Add(item);
            product.Stock -= itemReq.Quantity;
            product.UpdatedAt = DateTime.UtcNow;
        }

        invoice.TotalAmount = invoice.Items.Sum(i => i.TotalPrice);

        await _db.SaveChangesAsync();

        return Ok(MapToResponse(invoice));
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateInvoiceStatusRequest request)
    {
        var invoice = await _db.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null)
            return NotFound(new { message = "Invoice not found." });

        invoice.Status = request.Status;
        invoice.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(MapToResponse(invoice));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var invoice = await _db.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null)
            return NotFound(new { message = "Invoice not found." });

        if (invoice.Status != InvoiceStatus.Draft)
            return BadRequest(new { message = "Only Draft invoices can be deleted." });

        // Restore stock
        var productIds = invoice.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _db.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
        var productMap = products.ToDictionary(p => p.Id);
        foreach (var item in invoice.Items)
        {
            productMap[item.ProductId].Stock += item.Quantity;
            productMap[item.ProductId].UpdatedAt = DateTime.UtcNow;
        }

        _db.Invoices.Remove(invoice);
        await _db.SaveChangesAsync();

        return NoContent();
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
