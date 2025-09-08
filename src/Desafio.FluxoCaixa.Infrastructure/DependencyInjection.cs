using Desafio.FluxoCaixa.Application.Abstractions;
using Desafio.FluxoCaixa.Application.Identity.Commands;
using Desafio.FluxoCaixa.Infrastructure.Persistence;
using Desafio.FluxoCaixa.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Desafio.FluxoCaixa.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connStr = configuration.GetConnectionString("Postgres")
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__Postgres")
            ?? "Host=localhost;Database=fluxodb;Username=app;Password=app";

        services.AddDbContext<AppDbContext>(o =>
            o.UseNpgsql(connStr, b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
             .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));


        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        return services;
    }
}
