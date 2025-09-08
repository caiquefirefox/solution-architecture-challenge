using Desafio.FluxoCaixa.Application.Relatorios.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Desafio.FluxoCaixa.Relatorios.Api.Controllers;

[ApiController]
[Route("api/v1/relatorios")]
public class RelatoriosController(IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpGet("saldo-diario")]
    public async Task<IActionResult> SaldoDiario(
        [FromQuery] DateOnly de,
        [FromQuery] DateOnly ate,
        [FromQuery] decimal saldoInicial = 0,
        CancellationToken ct = default)
    {
        if (de > ate)
            return BadRequest(new { error = "Intervalo inválido: 'de' não pode ser maior que 'ate'.", de, ate });

        string? uidRaw =
              User.FindFirstValue("uid")
           ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
           ?? User.FindFirstValue("sub")
           ?? Request.Headers["X-UserId"].FirstOrDefault();

        if (!Guid.TryParse(uidRaw, out var uid))
            return BadRequest(new { error = "Usuário não identificado no token/header.", claim = uidRaw });

        var res = await mediator.Send(new SaldoDiarioQuery(uid, de, ate, saldoInicial), ct);
        return Ok(res);
    }
}
