using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Desafio.FluxoCaixa.Application.Abstractions;
using Desafio.FluxoCaixa.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Desafio.FluxoCaixa.Infrastructure.Security;

public sealed class JwtTokenGenerator(IConfiguration configuration) : IJwtTokenGenerator
{
    public (string AccessToken, DateTime ExpiresAtUtc) Generate(User user, IEnumerable<string>? roles = null, IDictionary<string,string>? extraClaims = null)
    {
        var issuer = configuration["Jwt:Issuer"] ?? "fluxo.caixa";
        var audience = configuration["Jwt:Audience"] ?? "fluxo.caixa.web";
        var key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key não configurado");
        var minutes = int.TryParse(configuration["Jwt:ExpiresMinutes"], out var m) ? m : 60;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("uid", user.Id.ToString())
        };
        if (roles != null) claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
        if (extraClaims != null) claims.AddRange(extraClaims.Select(kv => new Claim(kv.Key, kv.Value)));

        var expires = DateTime.UtcNow.AddMinutes(minutes);
        var token = new JwtSecurityToken(issuer: issuer, audience: audience, claims: claims, notBefore: DateTime.UtcNow.AddMinutes(-1), expires: expires, signingCredentials: credentials);
        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}
