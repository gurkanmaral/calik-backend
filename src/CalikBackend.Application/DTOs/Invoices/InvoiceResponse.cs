using CalikBackend.Domain.Entities;

namespace CalikBackend.Application.DTOs.Invoices;

public class InvoiceResponse
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public InvoiceStatus Status { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerAddress { get; set; }
    public string? CustomerTaxNumber { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? Notes { get; set; }
    public decimal TotalAmount { get; set; }
    public List<InvoiceItemResponse> Items { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
