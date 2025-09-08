using Desafio.FluxoCaixa.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Desafio.FluxoCaixa.Application.Lancamentos.Queries;

public sealed record ListarLancamentosQuery(Guid UserId, DateOnly? De = null, DateOnly? Ate = null) : IRequest<IReadOnlyList<LancamentoDto>>;

public sealed class ListarLancamentosHandler(IAppDbContext db) : IRequestHandler<ListarLancamentosQuery, IReadOnlyList<LancamentoDto>>
{
    public async Task<IReadOnlyList<LancamentoDto>> Handle(ListarLancamentosQuery request, CancellationToken ct)
    {
        var q = db.Lancamentos.AsNoTracking().Where(l => l.UserId == request.UserId);
        if (request.De is not null) q = q.Where(l => l.Data >= request.De);
        if (request.Ate is not null) q = q.Where(l => l.Data <= request.Ate);
        return await q.OrderBy(l => l.Data).ThenBy(l => l.Id)
            .Select(l => new LancamentoDto(l.Id, l.Data, l.Tipo, l.Valor, l.Descricao))
            .ToListAsync(ct);
    }
}
