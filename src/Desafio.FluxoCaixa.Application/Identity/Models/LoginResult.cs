namespace Desafio.FluxoCaixa.Application.Identity.Models;

public sealed record LoginResult(Guid UserId, string UserName, string Email);
