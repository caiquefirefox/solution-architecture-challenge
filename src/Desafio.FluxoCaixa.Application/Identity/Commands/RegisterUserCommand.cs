using Desafio.FluxoCaixa.Application.Abstractions;
using Desafio.FluxoCaixa.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Desafio.FluxoCaixa.Application.Identity.Commands;

public sealed record RegisterUserCommand(string UserName, string Email, string Password) : IRequest<Guid>;

public sealed class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().MinimumLength(3).MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}

public sealed class RegisterUserHandler(IAppDbContext db, IPasswordHasher hasher) : IRequestHandler<RegisterUserCommand, Guid>
{
    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var exists = await db.Users.AnyAsync(u => u.UserName == request.UserName || u.Email == request.Email, ct);
        if (exists) throw new InvalidOperationException("Usuário já existe.");
        var hash = hasher.Hash(request.Password);
        var user = new User(request.UserName, request.Email, hash);
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        return user.Id;
    }
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
