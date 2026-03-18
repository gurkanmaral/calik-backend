using System.ComponentModel.DataAnnotations;

namespace CalikBackend.Application.DTOs.Products;

public class UpdateStockRequest
{
    [Required]
    public int Stock { get; set; }
}
