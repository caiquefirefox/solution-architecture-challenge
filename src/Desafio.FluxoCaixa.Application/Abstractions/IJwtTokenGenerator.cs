using Desafio.FluxoCaixa.Domain.Entities;

namespace Desafio.FluxoCaixa.Application.Abstractions;

public interface IJwtTokenGenerator
{
    (string AccessToken, DateTime ExpiresAtUtc) Generate(User user, IEnumerable<string>? roles = null, IDictionary<string,string>? extraClaims = null);
}
