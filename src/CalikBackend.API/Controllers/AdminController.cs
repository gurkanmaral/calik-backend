using CalikBackend.Application.Features.Admin.Commands.AssignRole;
using CalikBackend.Application.Features.Admin.Commands.RemoveRole;
using CalikBackend.Application.Features.Admin.Queries.GetDashboard;
using CalikBackend.Application.Features.Admin.Queries.GetRoles;
using CalikBackend.Application.Features.Admin.Queries.GetUserRoles;
using CalikBackend.Application.Features.Admin.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalikBackend.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly ISender _sender;

    public AdminController(ISender sender) => _sender = sender;

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
        => Ok(await _sender.Send(new GetDashboardQuery()));

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
        => Ok(await _sender.Send(new GetRolesQuery()));

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
        => Ok(await _sender.Send(new GetUsersQuery()));

    [HttpPost("users/{userId}/roles")]
    public async Task<IActionResult> AssignRole(string userId, [FromBody] RoleRequest request)
    {
        await _sender.Send(new AssignRoleCommand(userId, request.Role));
        return Ok(new { message = $"Role '{request.Role}' assigned." });
    }

    [HttpDelete("users/{userId}/roles")]
    public async Task<IActionResult> RemoveRole(string userId, [FromBody] RoleRequest request)
    {
        await _sender.Send(new RemoveRoleCommand(userId, request.Role));
        return Ok(new { message = $"Role '{request.Role}' removed." });
    }

    [HttpGet("users/{userId}/roles")]
    public async Task<IActionResult> GetUserRoles(string userId)
        => Ok(await _sender.Send(new GetUserRolesQuery(userId)));
}

public record RoleRequest(string Role);
