namespace CalikBackend.Domain.Entities;

public class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? CountryCode { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public decimal Balance { get; set; } = 0;  // positive = they owe me, negative = I owe them
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
