using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Application.DTOs.Auth;
using CalikBackend.Application.Interfaces;
using CalikBackend.Application.Repositories.Interfaces;
using CalikBackend.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace CalikBackend.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthRepository _authRepo;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _config;

    public RefreshTokenCommandHandler(
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

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await _authRepo.GetActiveRefreshTokenAsync(request.Token, cancellationToken);
        if (token == null || !token.IsActive)
            throw new BadRequestException("Invalid or expired refresh token.");

        token.RevokedAt = DateTime.UtcNow;

        var user = await _userManager.FindByIdAsync(token.UserId)
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

        await _authRepo.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes)
        };
    }
}
