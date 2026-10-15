# ---------- Etapa 1: compilar y publicar ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restaurar dependencias primero (se cachea mientras el .csproj no cambie)
COPY BancoPanda.Api/BancoPanda.Api.csproj BancoPanda.Api/
RUN dotnet restore BancoPanda.Api/BancoPanda.Api.csproj

# Copiar el resto del código y publicar
COPY BancoPanda.Api/ BancoPanda.Api/
RUN dotnet publish BancoPanda.Api/BancoPanda.Api.csproj \
    -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---------- Etapa 2: imagen final (solo runtime) ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true \
    Database__AplicarMigraciones=true

COPY --from=build /app/publish .

# Render inyecta la variable PORT; si no existe se usa 8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["sh", "-c", "ASPNETCORE_HTTP_PORTS=${PORT:-8080} exec dotnet BancoPanda.Api.dll"]
