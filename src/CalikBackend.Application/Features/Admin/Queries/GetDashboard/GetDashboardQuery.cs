using CalikBackend.Application.DTOs.Dashboard;
using MediatR;

namespace CalikBackend.Application.Features.Admin.Queries.GetDashboard;

public record GetDashboardQuery : IRequest<DashboardResponse>;
