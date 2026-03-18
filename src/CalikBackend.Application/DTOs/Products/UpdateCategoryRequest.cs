using System.ComponentModel.DataAnnotations;

namespace CalikBackend.Application.DTOs.Products;

public class UpdateCategoryRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}
