using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Application.DTOs.Auth;
using CalikBackend.Application.Interfaces;
using CalikBackend.Application.Repositories.Interfaces;
using CalikBackend.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace CalikBackend.Application.Features.Auth.Commands.VerifyOtp;

public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, AuthResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthRepository _authRepo;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _config;

    public VerifyOtpCommandHandler(
        UserManager<ApplicationUser> userManager,
        IAuthRepository authRepo,
        IJwtService jwtService,
        IConfiguration config)
    {
        _userManager = userManager;
        _authRepo = authRepo;
        _jwtService = jwtService;
        _config = config;
    }

    public async Task<AuthResponse> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        var otp = await _authRepo.GetActiveOtpAsync(request.UserId, request.Code, cancellationToken);
        if (otp == null || otp.ExpiresAt < DateTime.UtcNow)
            throw new BadRequestException("Invalid or expired OTP.");

        _authRepo.RemoveOtp(otp);

        var user = await _userManager.FindByIdAsync(request.UserId)
            ?? throw new NotFoundException("User not found.");

        var roles = await _userManager.GetRolesAsync(user);

        var expiryMinutes = int.Parse(_config["Jwt:AccessTokenExpiryMinutes"]!);
        var refreshExpiryDays = int.Parse(_config["Jwt:RefreshTokenExpiryDays"]!);

        var accessToken = _jwtService.GenerateAccessToken(user, roles);
        var refreshTokenValue = _jwtService.GenerateRefreshToken();

        await _authRepo.AddRefreshTokenAsync(new CalikBackend.Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshExpiryDays)
        }, cancellationToken);

        // Single SaveChanges — removes OTP and adds refresh token atomically
        await _authRepo.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes)
        };
    }
}
