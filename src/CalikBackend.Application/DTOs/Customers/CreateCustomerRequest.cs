using System.ComponentModel.DataAnnotations;

namespace CalikBackend.Application.DTOs.Customers;

public class CreateCustomerRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? CountryCode { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public decimal Balance { get; set; } = 0;
}
