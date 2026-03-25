using CalikBackend.Domain.Entities;

namespace CalikBackend.Application.Repositories.Interfaces;

public interface IAuthRepository
{
    Task<UserOtp?> GetActiveOtpAsync(string userId, string code, CancellationToken ct);
    Task AddOtpAsync(UserOtp otp, CancellationToken ct);
    Task RemoveExistingOtpsAsync(string userId, CancellationToken ct);
    void RemoveOtp(UserOtp otp);
    Task<RefreshToken?> GetActiveRefreshTokenAsync(string token, CancellationToken ct);
    Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
