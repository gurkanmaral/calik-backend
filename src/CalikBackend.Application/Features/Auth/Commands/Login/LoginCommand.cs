using MediatR;

namespace CalikBackend.Application.Features.Auth.Commands.Login;

public record LoginResult(string UserId, string OtpCode);

public record LoginCommand(string Email, string Password, bool IsDevelopment) : IRequest<LoginResult>;
