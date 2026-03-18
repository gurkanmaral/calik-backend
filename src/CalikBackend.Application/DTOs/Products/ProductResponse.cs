namespace CalikBackend.Application.DTOs.Products;

public class ProductResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public decimal? Price { get; set; }
    public int Stock { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
