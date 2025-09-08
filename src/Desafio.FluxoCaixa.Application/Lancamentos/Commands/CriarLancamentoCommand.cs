using Desafio.FluxoCaixa.Application.Abstractions;
using Desafio.FluxoCaixa.Domain.Entities;
using Desafio.FluxoCaixa.Domain.Enums;
using FluentValidation;
using MediatR;

namespace Desafio.FluxoCaixa.Application.Lancamentos.Commands;

public sealed record CriarLancamentoCommand(Guid UserId, DateOnly Data, TipoLancamento Tipo, decimal Valor, string Descricao) : IRequest<Guid>;

public sealed class CriarLancamentoValidator : AbstractValidator<CriarLancamentoCommand>
{
    public CriarLancamentoValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Valor).GreaterThan(0);
        RuleFor(x => x.Descricao).NotEmpty().MaximumLength(200);
    }
}

public sealed class CriarLancamentoHandler(IAppDbContext db) : IRequestHandler<CriarLancamentoCommand, Guid>
{
    public async Task<Guid> Handle(CriarLancamentoCommand request, CancellationToken ct)
    {
        var e = new Lancamento(request.UserId, request.Data, request.Tipo, request.Valor, request.Descricao);
        db.Lancamentos.Add(e);
        await db.SaveChangesAsync(ct);
        return e.Id;
    }
}
