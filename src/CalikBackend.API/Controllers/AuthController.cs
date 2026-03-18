using System.Security.Claims;
using CalikBackend.Application.DTOs.Auth;
using CalikBackend.Application.Interfaces;
using CalikBackend.Domain.Entities;
using CalikBackend.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace CalikBackend.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _db;
    private readonly IJwtService _jwtService;
    private readonly ISmsService _smsService;
    private readonly IConfiguration _config;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        AppDbContext db,
        IJwtService jwtService,
        ISmsService smsService,
        IConfiguration config)
    {
        _userManager = userManager;
        _db = db;
        _jwtService = jwtService;
        _smsService = smsService;
        _config = config;
    }

    [HttpPost("register")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            PhoneNumber = request.PhoneNumber,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, "User");

        return Ok(new { message = "Registration successful." });
    }

    [HttpPost("login")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized(new { message = "Invalid credentials." });

        // Remove existing OTPs for this user
        var existingOtps = _db.UserOtps.Where(o => o.UserId == user.Id);
        _db.UserOtps.RemoveRange(existingOtps);

        var otp = new UserOtp
        {
            UserId = user.Id,
            Code = GenerateOtpCode(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(5)
        };

        _db.UserOtps.Add(otp);
        await _db.SaveChangesAsync();

        await _smsService.SendOtpAsync(user.PhoneNumber!, otp.Code);

        var isDev = HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment();
        return Ok(new
        {
            userId = user.Id,
            message = "OTP sent to your phone number.",
            devOtp = isDev ? otp.Code : null
        });
    }

    [HttpPost("verify-otp")]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        var otp = await _db.UserOtps
            .FirstOrDefaultAsync(o => o.UserId == request.UserId && o.Code == request.Code);

        if (otp == null || otp.ExpiresAt < DateTime.UtcNow)
            return BadRequest(new { message = "Invalid or expired OTP." });

        _db.UserOtps.Remove(otp);
        await _db.SaveChangesAsync();

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return NotFound(new { message = "User not found." });

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(await IssueTokens(user, roles));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var token = await _db.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken);

        if (token == null || !token.IsActive)
            return Unauthorized(new { message = "Invalid or expired refresh token." });

        token.RevokedAt = DateTime.UtcNow;

        var user = await _userManager.FindByIdAsync(token.UserId);
        if (user == null)
            return NotFound(new { message = "User not found." });

        var roles = await _userManager.GetRolesAsync(user);
        var response = await IssueTokens(user, roles);

        await _db.SaveChangesAsync();
        return Ok(response);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);
        if (user == null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new
        {
            user.Id,
            user.Email,
            user.PhoneNumber,
            user.FirstName,
            user.LastName,
            user.ProfilePhotoUrl,
            user.Bio,
            user.DateOfBirth,
            user.Address,
            user.City,
            user.Country,
            roles
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
    {
        var token = await _db.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken);

        if (token == null || !token.IsActive)
            return BadRequest(new { message = "Token not found or already revoked." });

        token.RevokedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(new { message = "Logged out successfully." });
    }

    private async Task<AuthResponse> IssueTokens(ApplicationUser user, IList<string> roles)
    {
        var expiryMinutes = int.Parse(_config["Jwt:AccessTokenExpiryMinutes"]!);
        var accessToken = _jwtService.GenerateAccessToken(user, roles);
        var refreshTokenValue = _jwtService.GenerateRefreshToken();
        var refreshExpiryDays = int.Parse(_config["Jwt:RefreshTokenExpiryDays"]!);

        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshExpiryDays)
        });

        await _db.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes)
        };
    }

    private static string GenerateOtpCode()
    {
        return Random.Shared.Next(100000, 999999).ToString();
    }
}
