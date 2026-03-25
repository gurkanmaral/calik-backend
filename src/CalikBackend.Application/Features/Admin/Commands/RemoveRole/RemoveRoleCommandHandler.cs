using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CalikBackend.Application.Features.Admin.Commands.RemoveRole;

public class RemoveRoleCommandHandler : IRequestHandler<RemoveRoleCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RemoveRoleCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId)
            ?? throw new NotFoundException("User not found.");

        if (!await _userManager.IsInRoleAsync(user, request.Role))
            throw new BadRequestException("User does not have this role.");

        await _userManager.RemoveFromRoleAsync(user, request.Role);
    }
}
