using MediatR;

namespace CalikBackend.Application.Features.Admin.Commands.RemoveRole;

public record RemoveRoleCommand(string UserId, string Role) : IRequest;
