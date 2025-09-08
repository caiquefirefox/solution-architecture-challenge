using Desafio.FluxoCaixa.Domain.Enums;

namespace Desafio.FluxoCaixa.Domain.Entities;

public sealed class Lancamento
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public DateOnly Data { get; private set; }
    public TipoLancamento Tipo { get; private set; }
    public decimal Valor { get; private set; }
    public string Descricao { get; private set; }

    public Lancamento(Guid userId, DateOnly data, TipoLancamento tipo, decimal valor, string descricao)
    {
        if (valor <= 0) throw new ArgumentException("Valor deve ser > 0");
        if (string.IsNullOrWhiteSpace(descricao)) throw new ArgumentException("Descrição obrigatória");

        UserId = userId;
        Data = data;
        Tipo = tipo;
        Valor = valor;
        Descricao = descricao;
    }

    private Lancamento() => Descricao = string.Empty;
}
