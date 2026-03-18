using System.ComponentModel.DataAnnotations;

namespace CalikBackend.Application.DTOs.Invoices;

public class InvoiceItemRequest
{
    [Required] public Guid ProductId { get; set; }
    [Required][Range(1, int.MaxValue)] public int Quantity { get; set; }
}
