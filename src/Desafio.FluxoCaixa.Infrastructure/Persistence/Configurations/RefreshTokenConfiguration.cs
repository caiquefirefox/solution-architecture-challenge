using Desafio.FluxoCaixa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desafio.FluxoCaixa.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> e)
    {
        e.ToTable("refresh_tokens");
        e.HasKey(rt => rt.Id);
        e.Property(rt => rt.TokenHash).IsRequired();
        e.Property(rt => rt.ExpiresAt).IsRequired();
        e.HasIndex(rt => rt.TokenHash).IsUnique();
        e.HasIndex(rt => rt.UserId);
    }
}
