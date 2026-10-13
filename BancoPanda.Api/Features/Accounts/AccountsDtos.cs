using BancoPanda.Api.Entities;

namespace BancoPanda.Api.Features.Accounts;

public record CuentaResponse(
    Guid CuentaId,
    string NumeroCuenta,
    TipoCuenta Tipo,
    decimal Saldo,
    string Moneda);
