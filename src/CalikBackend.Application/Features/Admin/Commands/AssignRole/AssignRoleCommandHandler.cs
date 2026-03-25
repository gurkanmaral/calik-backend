using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CalikBackend.Application.Features.Admin.Commands.AssignRole;

public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AssignRoleCommandHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId)
            ?? throw new NotFoundException("User not found.");

        if (!await _roleManager.RoleExistsAsync(request.Role))
            throw new BadRequestException($"Role '{request.Role}' does not exist.");

        if (await _userManager.IsInRoleAsync(user, request.Role))
            throw new BadRequestException("User already has this role.");

        await _userManager.AddToRoleAsync(user, request.Role);
    }
}
