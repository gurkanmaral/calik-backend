using MediatR;

namespace CalikBackend.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string PhoneNumber,
    string FirstName,
    string LastName) : IRequest;
