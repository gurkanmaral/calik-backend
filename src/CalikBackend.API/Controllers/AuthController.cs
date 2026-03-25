using System.Security.Claims;
using CalikBackend.Application.DTOs.Auth;
using CalikBackend.Application.Features.Auth.Commands.Login;
using CalikBackend.Application.Features.Auth.Commands.Logout;
using CalikBackend.Application.Features.Auth.Commands.Register;
using CalikBackend.Application.Features.Auth.Commands.RefreshToken;
using CalikBackend.Application.Features.Auth.Commands.VerifyOtp;
using CalikBackend.Application.Features.Auth.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CalikBackend.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender) => _sender = sender;

    [HttpPost("register")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        await _sender.Send(new RegisterCommand(
            request.Email, request.Password, request.PhoneNumber,
            request.FirstName, request.LastName));
        return Ok(new { message = "Registration successful." });
    }

    [HttpPost("login")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var isDev = HttpContext.RequestServices
            .GetRequiredService<IWebHostEnvironment>().IsDevelopment();

        var result = await _sender.Send(new LoginCommand(request.Email, request.Password, isDev));

        return Ok(new
        {
            userId = result.UserId,
            message = "OTP sent to your phone number.",
            devOtp = isDev ? result.OtpCode : null
        });
    }

    [HttpPost("verify-otp")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        => Ok(await _sender.Send(new VerifyOtpCommand(request.UserId, request.Code)));

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        => Ok(await _sender.Send(new RefreshTokenCommand(request.RefreshToken)));

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await _sender.Send(new GetCurrentUserQuery(userId)));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
    {
        await _sender.Send(new LogoutCommand(request.RefreshToken));
        return Ok(new { message = "Logged out successfully." });
    }
}
