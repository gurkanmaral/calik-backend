using MediatR;

namespace CalikBackend.Application.Features.Admin.Queries.GetUsers;

public record UserDto(string Id, string? Email, string? FirstName, string? LastName, string? PhoneNumber);

public record GetUsersQuery : IRequest<List<UserDto>>;
