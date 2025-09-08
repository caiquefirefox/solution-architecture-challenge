# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ./Desafio.FluxoCaixa.Domain ./Desafio.FluxoCaixa.Domain
COPY ./Desafio.FluxoCaixa.Application ./Desafio.FluxoCaixa.Application
COPY ./Desafio.FluxoCaixa.Infrastructure ./Desafio.FluxoCaixa.Infrastructure
COPY ./Desafio.FluxoCaixa.Relatorios.Api ./Desafio.FluxoCaixa.Relatorios.Api
RUN dotnet restore ./Desafio.FluxoCaixa.Relatorios.Api/Desafio.FluxoCaixa.Relatorios.Api.csproj
RUN dotnet publish ./Desafio.FluxoCaixa.Relatorios.Api/Desafio.FluxoCaixa.Relatorios.Api.csproj -c Release -o /app/publish
# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Desafio.FluxoCaixa.Relatorios.Api.dll"]
