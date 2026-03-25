using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CalikBackend.Application.Features.Admin.Queries.GetUserRoles;

public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, IList<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUserRolesQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IList<string>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId)
            ?? throw new NotFoundException("User not found.");

        return await _userManager.GetRolesAsync(user);
    }
}
