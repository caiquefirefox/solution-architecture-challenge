using System.Security.Cryptography;
using System.Text;
using Desafio.FluxoCaixa.Application.Abstractions;
using Desafio.FluxoCaixa.Domain.Entities;
using Desafio.FluxoCaixa.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Desafio.FluxoCaixa.Infrastructure.Security;

public sealed class RefreshTokenService(AppDbContext db) : IRefreshTokenService
{
    private static string Hash(string raw)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(bytes);
    }
    private static string NewRawToken()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    public async Task<string> IssueAsync(User user, string? userAgent, string? ip, CancellationToken ct)
    {
        var raw = NewRawToken();
        var hash = Hash(raw);
        var entity = new RefreshToken(user.Id, hash, DateTime.UtcNow.AddDays(7), userAgent, ip);
        await db.RefreshTokens.AddAsync(entity, ct);
        await db.SaveChangesAsync(ct);
        return raw;
    }

    public async Task<(string NewToken, User User)> RotateAsync(string currentToken, string? userAgent, string? ip, CancellationToken ct)
    {
        var hash = Hash(currentToken);
        var existing = await db.RefreshTokens.AsTracking().FirstOrDefaultAsync(t => t.TokenHash == hash, ct);
        if (existing is null) throw new UnauthorizedAccessException("Refresh token inválido");
        if (existing.RevokedAt is not null || existing.ExpiresAt <= DateTime.UtcNow) throw new UnauthorizedAccessException("Refresh token expirado/revogado");

        var user = await db.Users.AsNoTracking().FirstAsync(u => u.Id == existing.UserId, ct);

        var newRaw = NewRawToken();
        var newHash = Hash(newRaw);
        existing.Revoke(newHash);
        var next = new RefreshToken(existing.UserId, newHash, existing.ExpiresAt.AddDays(7), userAgent, ip);
        await db.RefreshTokens.AddAsync(next, ct);
        await db.SaveChangesAsync(ct);
        return (newRaw, user);
    }

    public async Task<bool> RevokeAsync(string token, CancellationToken ct)
    {
        var hash = Hash(token);
        var existing = await db.RefreshTokens.AsTracking().FirstOrDefaultAsync(t => t.TokenHash == hash, ct);
        if (existing is null || existing.RevokedAt is not null) return false;
        existing.Revoke();
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<int> RevokeAllForUserAsync(Guid userId, CancellationToken ct)
    {
        var items = await db.RefreshTokens.Where(t => t.UserId == userId && t.RevokedAt == null).ToListAsync(ct);
        foreach (var i in items) i.Revoke();
        await db.SaveChangesAsync(ct);
        return items.Count;
    }
}
