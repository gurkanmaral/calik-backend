using CalikBackend.Application.Common.Exceptions;
using CalikBackend.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CalikBackend.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RegisterCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            PhoneNumber = request.PhoneNumber,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new BadRequestException(errors);
        }

        await _userManager.AddToRoleAsync(user, "User");
    }
}
