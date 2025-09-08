using Desafio.FluxoCaixa.Application.Abstractions;
using Desafio.FluxoCaixa.Application.Identity.Commands;
using Desafio.FluxoCaixa.Application.Identity.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Desafio.FluxoCaixa.Application.Identity;

public sealed record LoginCommand(string UserNameOrEmail, string Password) : IRequest<LoginResult>;

public sealed class LoginHandler(IAppDbContext db, IPasswordHasher hasher) : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await db.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == request.UserNameOrEmail || u.Email == request.UserNameOrEmail, ct);
        if (user is null) throw new UnauthorizedAccessException("Credenciais inválidas");
        if (!hasher.Verify(request.Password, user.PasswordHash)) throw new UnauthorizedAccessException("Credenciais inválidas");
        return new LoginResult(user.Id, user.UserName, user.Email);
    }
}
