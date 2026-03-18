using System.ComponentModel.DataAnnotations;

namespace CalikBackend.Application.DTOs.Products;

public class UpdateProductRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? ImageUrl { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required]
    public int Stock { get; set; }
    public string Unit { get; set; } = "adet";
    [Required]
    public Guid CategoryId { get; set; }
    public bool IsActive { get; set; } = true;
}
