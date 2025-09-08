using Desafio.FluxoCaixa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desafio.FluxoCaixa.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> e)
    {
        e.ToTable("users");
        e.HasKey(u => u.Id);
        e.Property(u => u.UserName).IsRequired().HasMaxLength(50);
        e.Property(u => u.Email).IsRequired().HasMaxLength(200);
        e.Property(u => u.PasswordHash).IsRequired();
        e.HasIndex(u => u.UserName).IsUnique();
        e.HasIndex(u => u.Email).IsUnique();
    }
}
