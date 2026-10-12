using BancoPanda.Api.Entities;

namespace BancoPanda.Api.Features.Accounts;

public record CuentaResponse(
    Guid CuentaId,
    string NumeroCuenta,
    TipoCuenta Tipo,
    decimal Saldo,
    string Moneda);

public record TarjetaResponse(
    Guid TarjetaId,
    Guid CuentaId,
    string UltimosDigitos,
    MarcaTarjeta Marca,
    TipoTarjeta Tipo,
    DateOnly FechaExpiracion);

public record MovimientoResponse(
    Guid MovimientoId,
    Guid CuentaId,
    string Descripcion,
    decimal Monto,
    TipoMovimiento Tipo,
    CategoriaMovimiento Categoria,
    DateTime Fecha);

public record ResumenResponse(
    CuentaResponse Cuenta,
    IReadOnlyList<TarjetaResponse> Tarjetas,
    IReadOnlyList<MovimientoResponse> Movimientos);
