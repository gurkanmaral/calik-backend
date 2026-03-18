namespace CalikBackend.Domain.Entities;

public class UserOtp
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
