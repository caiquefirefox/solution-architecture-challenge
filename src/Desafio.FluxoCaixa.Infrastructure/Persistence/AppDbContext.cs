using Desafio.FluxoCaixa.Application.Abstractions;
using Desafio.FluxoCaixa.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Desafio.FluxoCaixa.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<Lancamento> Lancamentos => Set<Lancamento>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
