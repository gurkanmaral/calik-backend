using CalikBackend.Application.DTOs.Auth;
using MediatR;

namespace CalikBackend.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string Token) : IRequest<AuthResponse>;
