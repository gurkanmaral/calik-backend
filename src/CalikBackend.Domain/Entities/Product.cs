namespace CalikBackend.Domain.Entities;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Unit { get; set; } = "adet";
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid CategoryId { get; set; }
    public ProductCategory Category { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
