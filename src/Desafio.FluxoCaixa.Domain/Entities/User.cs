namespace Desafio.FluxoCaixa.Domain.Entities;

public sealed class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string UserName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public User(string userName, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentException("UserName obrigatório");
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email obrigatório");
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("Hash obrigatório");

        UserName = userName;
        Email = email;
        PasswordHash = passwordHash;
    }

    private User() => UserName = Email = PasswordHash = string.Empty;
}
