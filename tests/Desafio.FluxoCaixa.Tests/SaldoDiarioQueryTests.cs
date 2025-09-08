using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Desafio.FluxoCaixa.Application.Relatorios.Queries;
using Desafio.FluxoCaixa.Domain.Entities;
using Desafio.FluxoCaixa.Domain.Enums;
using Desafio.FluxoCaixa.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Desafio.FluxoCaixa.Tests;

public class SaldoDiarioQueryTests
{
    [Fact]
    public async Task Calcula_saldo_acumulado_corretamente()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new AppDbContext(options);

        var uid = Guid.NewGuid();
        db.Lancamentos.Add(new Lancamento(uid, new DateOnly(2025,9,1), TipoLancamento.Credito, 100, "Venda"));
        db.Lancamentos.Add(new Lancamento(uid, new DateOnly(2025,9,1), TipoLancamento.Debito, 40, "Compra"));
        db.Lancamentos.Add(new Lancamento(uid, new DateOnly(2025,9,2), TipoLancamento.Debito, 10, "Café"));
        await db.SaveChangesAsync();

        var handler = new SaldoDiarioHandler(db);
        var list = await handler.Handle(new SaldoDiarioQuery(uid, new DateOnly(2025,9,1), new DateOnly(2025,9,2), 0), CancellationToken.None);

        list.Should().HaveCount(2);
        list[0].SaldoDoDia.Should().Be(60);
        list[0].SaldoAcumulado.Should().Be(60);
        list[1].SaldoDoDia.Should().Be(-10);
        list[1].SaldoAcumulado.Should().Be(50);
    }
}
