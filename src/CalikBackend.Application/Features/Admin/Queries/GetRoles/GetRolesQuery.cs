using MediatR;

namespace CalikBackend.Application.Features.Admin.Queries.GetRoles;

public record RoleDto(string Id, string? Name);

public record GetRolesQuery : IRequest<List<RoleDto>>;
