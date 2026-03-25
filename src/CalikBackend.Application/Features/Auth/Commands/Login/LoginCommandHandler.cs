using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Application.Interfaces;
using CalikBackend.Application.Repositories.Interfaces;
using CalikBackend.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CalikBackend.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthRepository _authRepo;
    private readonly ISmsService _smsService;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        IAuthRepository authRepo,
        ISmsService smsService)
    {
        _userManager = userManager;
        _authRepo = authRepo;
        _smsService = smsService;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new BadRequestException("Invalid credentials.");

        await _authRepo.RemoveExistingOtpsAsync(user.Id, cancellationToken);

        var otp = new UserOtp
        {
            UserId = user.Id,
            Code = GenerateOtpCode(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(5)
        };

        await _authRepo.AddOtpAsync(otp, cancellationToken);
        await _authRepo.SaveChangesAsync(cancellationToken);

        await _smsService.SendOtpAsync(user.PhoneNumber!, otp.Code);

        return new LoginResult(user.Id, otp.Code);
    }

    private static string GenerateOtpCode()
        => Random.Shared.Next(100000, 999999).ToString();
}
