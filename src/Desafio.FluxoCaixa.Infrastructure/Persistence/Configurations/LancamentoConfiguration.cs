using Desafio.FluxoCaixa.Domain.Entities;
using Desafio.FluxoCaixa.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desafio.FluxoCaixa.Infrastructure.Persistence.Configurations;

public class LancamentoConfiguration : IEntityTypeConfiguration<Lancamento>
{
    public void Configure(EntityTypeBuilder<Lancamento> e)
    {
        e.ToTable("lancamentos");
        e.HasKey(l => l.Id);
        e.Property(l => l.UserId).IsRequired();
        e.Property(l => l.Data).HasColumnType("date").IsRequired();
        e.Property(l => l.Tipo).HasConversion<int>().IsRequired();
        e.Property(l => l.Valor).HasColumnType("numeric(14,2)").IsRequired();
        e.Property(l => l.Descricao).IsRequired();
        e.HasIndex(l => new { l.UserId, l.Data });
    }
}
