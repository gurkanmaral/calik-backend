namespace CalikBackend.Domain.Entities;

public class Invoice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerAddress { get; set; }
    public string? CustomerTaxNumber { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }

    public string? Notes { get; set; }
    public decimal TotalAmount { get; set; }

    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
