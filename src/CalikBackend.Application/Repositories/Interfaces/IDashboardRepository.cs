using CalikBackend.Application.DTOs.Dashboard;

namespace CalikBackend.Application.Repositories.Interfaces;

public interface IDashboardRepository
{
    Task<DashboardResponse> GetDashboardAsync(DateTime from, CancellationToken ct);
}
