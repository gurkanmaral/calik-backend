using CalikBackend.Application.DTOs.Dashboard;
using CalikBackend.Application.Repositories.Interfaces;
using MediatR;

namespace CalikBackend.Application.Features.Admin.Queries.GetDashboard;

public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardResponse>
{
    private readonly IDashboardRepository _repo;

    public GetDashboardQueryHandler(IDashboardRepository repo) => _repo = repo;

    public Task<DashboardResponse> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
        => _repo.GetDashboardAsync(DateTime.UtcNow.AddDays(-7), cancellationToken);
}
