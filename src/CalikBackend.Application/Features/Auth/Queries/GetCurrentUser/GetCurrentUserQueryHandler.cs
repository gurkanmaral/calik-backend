using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CalikBackend.Application.Features.Auth.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public GetCurrentUserQueryHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<CurrentUserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId)
            ?? throw new NotFoundException("User not found.");

        var roles = await _userManager.GetRolesAsync(user);

        return new CurrentUserDto(
            user.Id,
            user.Email,
            user.PhoneNumber,
            user.FirstName,
            user.LastName,
            user.ProfilePhotoUrl,
            user.Bio,
            user.DateOfBirth,
            user.Address,
            user.City,
            user.Country,
            roles);
    }
}
