using MediatR;

namespace CalikBackend.Application.Features.Admin.Commands.AssignRole;

public record AssignRoleCommand(string UserId, string Role) : IRequest;
