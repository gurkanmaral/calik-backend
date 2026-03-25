using MediatR;

namespace CalikBackend.Application.Features.Admin.Queries.GetUserRoles;

public record GetUserRolesQuery(string UserId) : IRequest<IList<string>>;
