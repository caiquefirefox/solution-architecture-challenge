using Desafio.FluxoCaixa.Domain.Enums;

namespace Desafio.FluxoCaixa.Application.Lancamentos.Queries;

public sealed record LancamentoDto(Guid Id, DateOnly Data, TipoLancamento Tipo, decimal Valor, string Descricao);
