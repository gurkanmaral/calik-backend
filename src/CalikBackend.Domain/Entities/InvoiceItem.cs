namespace CalikBackend.Domain.Entities;

public class InvoiceItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string ProductName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}
