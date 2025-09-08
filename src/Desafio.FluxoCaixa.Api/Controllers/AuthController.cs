using Desafio.FluxoCaixa.Application.Abstractions;
using Desafio.FluxoCaixa.Application.Identity;
using Desafio.FluxoCaixa.Application.Identity.Commands;
using Desafio.FluxoCaixa.Application.Identity.Dtos;
using Desafio.FluxoCaixa.Application.Identity.Models;
using Desafio.FluxoCaixa.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Desafio.FluxoCaixa.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(IMediator mediator, IJwtTokenGenerator jwt, IRefreshTokenService refreshSvc, AppDbContext db, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<Guid>> Register([FromBody] RegisterUserCommand cmd)
    {
        var id = await mediator.Send(cmd);
        return Created(string.Empty, new { id });
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenPairDto>> Login([FromBody] LoginCommand cmd)
    {
        try
        {
            var res = await mediator.Send(cmd);

            var user = await db.Users.AsNoTracking()
                .SingleAsync(u => u.Id == res.UserId, HttpContext.RequestAborted);

            var (access, exp) = jwt.Generate(user);

            var rawRefresh = await refreshSvc.IssueAsync(
                user,
                Request.Headers.UserAgent.ToString(),
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                HttpContext.RequestAborted
            );

            return Ok(new TokenPairDto(access, rawRefresh, (long)(exp - DateTime.UtcNow).TotalSeconds));
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(ex, "Falha de login para {user}", cmd.UserNameOrEmail);
            return Unauthorized();
        }
    }

    public sealed record RefreshRequest(string RefreshToken);

    [HttpPost("refresh")]
    public async Task<ActionResult<TokenPairDto>> Refresh([FromBody] RefreshRequest model)
    {
        try
        {
            var (newRaw, user) = await refreshSvc.RotateAsync(model.RefreshToken, Request.Headers.UserAgent.ToString(), HttpContext.Connection.RemoteIpAddress?.ToString(), HttpContext.RequestAborted);
            var (access, exp) = jwt.Generate(user);
            return Ok(new TokenPairDto(access, newRaw, (long)(exp - DateTime.UtcNow).TotalSeconds));
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(ex, "Refresh inválido");
            return Unauthorized();
        }
    }

    public sealed record LogoutRequest(string RefreshToken, bool AllDevices = false);

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest model)
    {
        if (model.AllDevices)
        {
            var uid = User.FindFirstValue("uid");
            if (uid is null) return Unauthorized();
            await refreshSvc.RevokeAllForUserAsync(Guid.Parse(uid), HttpContext.RequestAborted);
        }
        else
        {
            await refreshSvc.RevokeAsync(model.RefreshToken, HttpContext.RequestAborted);
        }
        return NoContent();
    }
}
