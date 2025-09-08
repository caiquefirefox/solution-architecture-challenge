using Desafio.FluxoCaixa.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Desafio.FluxoCaixa.Application.Lancamentos.Commands;

public sealed record RemoverLancamentoCommand(Guid UserId, Guid LancamentoId) : IRequest<bool>;

public sealed class RemoverLancamentoHandler(IAppDbContext db) : IRequestHandler<RemoverLancamentoCommand, bool>
{
    public async Task<bool> Handle(RemoverLancamentoCommand request, CancellationToken ct)
    {
        var e = await db.Lancamentos.FirstOrDefaultAsync(l => l.Id == request.LancamentoId && l.UserId == request.UserId, ct);
        if (e is null) return false;
        db.Lancamentos.Remove(e);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
