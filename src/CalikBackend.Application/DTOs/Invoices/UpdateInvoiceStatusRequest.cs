using System.ComponentModel.DataAnnotations;
using CalikBackend.Domain.Entities;

namespace CalikBackend.Application.DTOs.Invoices;

public class UpdateInvoiceStatusRequest
{
    [Required] public InvoiceStatus Status { get; set; }
}
