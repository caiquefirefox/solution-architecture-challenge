namespace Desafio.FluxoCaixa.Application.Identity.Dtos;

public sealed record TokenPairDto(string AccessToken, string RefreshToken, long ExpiresInSeconds);
