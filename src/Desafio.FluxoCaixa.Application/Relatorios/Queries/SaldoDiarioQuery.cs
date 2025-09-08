using Desafio.FluxoCaixa.Application.Abstractions;
using Desafio.FluxoCaixa.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Desafio.FluxoCaixa.Application.Relatorios.Queries;

public sealed record SaldoDiarioQuery(Guid UserId, DateOnly De, DateOnly Ate, decimal SaldoInicial = 0) : IRequest<IReadOnlyList<DailyBalanceDto>>;

public sealed class SaldoDiarioHandler(IAppDbContext db) : IRequestHandler<SaldoDiarioQuery, IReadOnlyList<DailyBalanceDto>>
{
    public async Task<IReadOnlyList<DailyBalanceDto>> Handle(SaldoDiarioQuery request, CancellationToken ct)
    {
        var lanc = await db.Lancamentos.AsNoTracking()
            .Where(l => l.UserId == request.UserId && l.Data >= request.De && l.Data <= request.Ate)
            .GroupBy(l => l.Data)
            .Select(g => new {
                Data = g.Key,
                Debitos = g.Where(x => x.Tipo == TipoLancamento.Debito).Sum(x => x.Valor),
                Creditos = g.Where(x => x.Tipo == TipoLancamento.Credito).Sum(x => x.Valor)
            })
            .OrderBy(x => x.Data)
            .ToListAsync(ct);

        var result = new List<DailyBalanceDto>();
        decimal acumulado = request.SaldoInicial;
        var dias = Enumerable.Range(0, (request.Ate.ToDateTime(TimeOnly.MinValue) - request.De.ToDateTime(TimeOnly.MinValue)).Days + 1)
                             .Select(i => request.De.AddDays(i));

        var dict = lanc.ToDictionary(x => x.Data, x => (x.Debitos, x.Creditos));
        foreach (var d in dias)
        {
            dict.TryGetValue(d, out var val);
            var debitos = val.Debitos;
            var creditos = val.Creditos;
            var saldoDia = creditos - debitos;
            acumulado += saldoDia;
            result.Add(new DailyBalanceDto(d, debitos, creditos, saldoDia, acumulado));
        }
        return result;
    }
}
