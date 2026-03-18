using System.ComponentModel.DataAnnotations;

namespace CalikBackend.Application.DTOs.Auth;

public class VerifyOtpRequest
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;
}
