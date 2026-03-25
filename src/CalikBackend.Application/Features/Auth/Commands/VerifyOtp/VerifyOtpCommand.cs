using CalikBackend.Application.DTOs.Auth;
using MediatR;

namespace CalikBackend.Application.Features.Auth.Commands.VerifyOtp;

public record VerifyOtpCommand(string UserId, string Code) : IRequest<AuthResponse>;
