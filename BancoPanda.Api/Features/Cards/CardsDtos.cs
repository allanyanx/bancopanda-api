using BancoPanda.Api.Entities;

namespace BancoPanda.Api.Features.Cards;

public record TarjetaResponse(
    Guid TarjetaId,
    string NumeroTarjeta,
    string UltimosDigitos,
    MarcaTarjeta Marca,
    TipoTarjeta Tipo,
    DateOnly FechaExpiracion,
    bool Activa,
    Guid CuentaId,
    string NumeroCuenta,
    decimal Saldo,
    string Moneda);
