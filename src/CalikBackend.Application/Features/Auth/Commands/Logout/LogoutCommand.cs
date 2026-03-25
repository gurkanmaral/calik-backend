using MediatR;

namespace CalikBackend.Application.Features.Auth.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest;
