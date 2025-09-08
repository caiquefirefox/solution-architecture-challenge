using System.Security.Claims;
using Desafio.FluxoCaixa.Application.Lancamentos.Commands;
using Desafio.FluxoCaixa.Application.Lancamentos.Queries;
using Desafio.FluxoCaixa.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Desafio.FluxoCaixa.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class LancamentosController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LancamentoDto>>> Listar([FromQuery] DateOnly? de, [FromQuery] DateOnly? ate)
    {
        var uid = Guid.Parse(User.FindFirstValue("uid")!);
        var result = await mediator.Send(new ListarLancamentosQuery(uid, de, ate));
        return Ok(result);
    }

    public sealed record CriarLancRequest(DateOnly Data, TipoLancamento Tipo, decimal Valor, string Descricao);

    [HttpPost]
    public async Task<ActionResult<Guid>> Criar([FromBody] CriarLancRequest req)
    {
        var uid = Guid.Parse(User.FindFirstValue("uid")!);
        var id = await mediator.Send(new CriarLancamentoCommand(uid, req.Data, req.Tipo, req.Valor, req.Descricao));
        return CreatedAtAction(nameof(Listar), new { id }, new { id });
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Remover(Guid id)
    {
        var uid = Guid.Parse(User.FindFirstValue("uid")!);
        var ok = await mediator.Send(new RemoverLancamentoCommand(uid, id));
        return ok ? NoContent() : NotFound();
    }
}
