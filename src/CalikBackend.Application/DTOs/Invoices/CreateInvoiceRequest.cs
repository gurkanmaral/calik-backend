using System.ComponentModel.DataAnnotations;

namespace CalikBackend.Application.DTOs.Invoices;

public class CreateInvoiceRequest
{
    public DateTime? InvoiceDate { get; set; }
    [Required] public string CustomerName { get; set; } = string.Empty;
    public string? CustomerAddress { get; set; }
    public string? CustomerTaxNumber { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? Notes { get; set; }
    [Required][MinLength(1)] public List<InvoiceItemRequest> Items { get; set; } = [];
}
