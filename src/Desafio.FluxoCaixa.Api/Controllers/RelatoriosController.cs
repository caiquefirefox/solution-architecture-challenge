using System.Security.Claims;
using Desafio.FluxoCaixa.Application.Relatorios.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Desafio.FluxoCaixa.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class RelatoriosController(IMediator mediator) : ControllerBase
{
    [HttpGet("saldo-diario")]
    public async Task<ActionResult<IReadOnlyList<DailyBalanceDto>>> SaldoDiario([FromQuery] DateOnly de, [FromQuery] DateOnly ate, [FromQuery] decimal? saldoInicial)
    {
        var uid = Guid.Parse(User.FindFirstValue("uid")!);
        var res = await mediator.Send(new SaldoDiarioQuery(uid, de, ate, saldoInicial ?? 0));
        return Ok(res);
    }
}
