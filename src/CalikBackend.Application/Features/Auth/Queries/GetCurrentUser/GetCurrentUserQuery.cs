using MediatR;

namespace CalikBackend.Application.Features.Auth.Queries.GetCurrentUser;

public record CurrentUserDto(
    string Id,
    string? Email,
    string? PhoneNumber,
    string? FirstName,
    string? LastName,
    string? ProfilePhotoUrl,
    string? Bio,
    DateTime? DateOfBirth,
    string? Address,
    string? City,
    string? Country,
    IList<string> Roles);

public record GetCurrentUserQuery(string UserId) : IRequest<CurrentUserDto>;
