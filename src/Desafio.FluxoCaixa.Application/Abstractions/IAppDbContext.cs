using Desafio.FluxoCaixa.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Desafio.FluxoCaixa.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<Lancamento> Lancamentos { get; }
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
