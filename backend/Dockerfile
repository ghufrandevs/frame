# ===== Stage 1: build and publish with the full SDK =====
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Project files first: "restore" stays cached until a .csproj changes.
COPY Frame.Domain/Frame.Domain.csproj Frame.Domain/
COPY Frame.Application/Frame.Application.csproj Frame.Application/
COPY Frame.Infrastructure/Frame.Infrastructure.csproj Frame.Infrastructure/
COPY Frame.Api/Frame.Api.csproj Frame.Api/
RUN dotnet restore Frame.Api/Frame.Api.csproj

# Then the source code.
COPY . .
RUN dotnet publish Frame.Api/Frame.Api.csproj -c Release -o /app/publish --no-restore

# ===== Stage 2: small runtime image, no SDK, no source code =====
# The aspnet image already listens on port 8080 (ASPNETCORE_HTTP_PORTS).
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

# Serilog writes to /app/logs: create it for the non-root user before dropping root.
RUN mkdir -p /app/logs && chown $APP_UID /app/logs
USER $APP_UID

ENTRYPOINT ["dotnet", "Frame.Api.dll"]