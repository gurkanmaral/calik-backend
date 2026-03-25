using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Application.Repositories.Interfaces;
using MediatR;

namespace CalikBackend.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IAuthRepository _authRepo;

    public LogoutCommandHandler(IAuthRepository authRepo) => _authRepo = authRepo;

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var token = await _authRepo.GetActiveRefreshTokenAsync(request.RefreshToken, cancellationToken);
        if (token == null || !token.IsActive)
            throw new BadRequestException("Token not found or already revoked.");

        token.RevokedAt = DateTime.UtcNow;
        await _authRepo.SaveChangesAsync(cancellationToken);
    }
}
