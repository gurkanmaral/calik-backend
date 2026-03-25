using CalikBackend.Application.Repositories.Interfaces;
using CalikBackend.Domain.Entities;
using CalikBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CalikBackend.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _db;

    public AuthRepository(AppDbContext db) => _db = db;

    public Task<UserOtp?> GetActiveOtpAsync(string userId, string code, CancellationToken ct)
        => _db.UserOtps.FirstOrDefaultAsync(o => o.UserId == userId && o.Code == code, ct);

    public async Task AddOtpAsync(UserOtp otp, CancellationToken ct)
        => await _db.UserOtps.AddAsync(otp, ct);

    public async Task RemoveExistingOtpsAsync(string userId, CancellationToken ct)
    {
        var existing = _db.UserOtps.Where(o => o.UserId == userId);
        _db.UserOtps.RemoveRange(existing);
        await Task.CompletedTask;
    }

    public void RemoveOtp(UserOtp otp)
        => _db.UserOtps.Remove(otp);

    public Task<RefreshToken?> GetActiveRefreshTokenAsync(string token, CancellationToken ct)
        => _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token, ct);

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken ct)
        => await _db.RefreshTokens.AddAsync(refreshToken, ct);

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}
