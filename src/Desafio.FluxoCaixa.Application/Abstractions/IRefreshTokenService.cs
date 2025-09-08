using Desafio.FluxoCaixa.Domain.Entities;

namespace Desafio.FluxoCaixa.Application.Abstractions;

public interface IRefreshTokenService
{
    Task<string> IssueAsync(User user, string? userAgent, string? ip, CancellationToken ct);
    Task<(string NewToken, User User)> RotateAsync(string currentToken, string? userAgent, string? ip, CancellationToken ct);
    Task<bool> RevokeAsync(string token, CancellationToken ct);
    Task<int> RevokeAllForUserAsync(Guid userId, CancellationToken ct);
}
