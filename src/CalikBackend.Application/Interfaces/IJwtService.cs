using CalikBackend.Domain.Entities;

namespace CalikBackend.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(ApplicationUser user, IList<string> roles);
    string GenerateRefreshToken();
}
