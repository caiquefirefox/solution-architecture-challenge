namespace Desafio.FluxoCaixa.Domain.Entities;

public sealed class RefreshToken
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByTokenHash { get; private set; }
    public string? UserAgent { get; private set; }
    public string? IpAddress { get; private set; }
    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;

    public RefreshToken(Guid userId, string tokenHash, DateTime expiresAt, string? userAgent, string? ip)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        UserAgent = userAgent;
        IpAddress = ip;
    }
    public void Revoke(string? replacedByHash = null)
    {
        RevokedAt = DateTime.UtcNow;
        ReplacedByTokenHash = replacedByHash;
    }
    private RefreshToken() {}
}
