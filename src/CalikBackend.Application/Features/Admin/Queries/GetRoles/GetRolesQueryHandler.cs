using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CalikBackend.Application.Features.Admin.Queries.GetRoles;

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, List<RoleDto>>
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public GetRolesQueryHandler(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<List<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        => await _roleManager.Roles
            .Select(r => new RoleDto(r.Id, r.Name))
            .ToListAsync(cancellationToken);
}
