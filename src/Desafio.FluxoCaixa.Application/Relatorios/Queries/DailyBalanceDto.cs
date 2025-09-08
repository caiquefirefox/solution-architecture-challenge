namespace Desafio.FluxoCaixa.Application.Relatorios.Queries;

public sealed record DailyBalanceDto(DateOnly Data, decimal Debitos, decimal Creditos, decimal SaldoDoDia, decimal SaldoAcumulado);
