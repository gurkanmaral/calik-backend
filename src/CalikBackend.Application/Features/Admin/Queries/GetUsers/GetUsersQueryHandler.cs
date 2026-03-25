using CalikBackend.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CalikBackend.Application.Features.Admin.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetUsersQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        => await _userManager.Users
            .Select(u => new UserDto(u.Id, u.Email, u.FirstName, u.LastName, u.PhoneNumber))
            .ToListAsync(cancellationToken);
}
