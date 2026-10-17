using BancoPanda.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace BancoPanda.Api.Features.Cards;

public class CardsService(AppDbContext db)
{
    public Task<List<TarjetaResponse>> ListarAsync(Guid usuarioId, CancellationToken ct) =>
        db.Tarjetas
            .Where(t => t.Cuenta.UsuarioId == usuarioId)
            .OrderByDescending(t => t.Activa)
            .ThenBy(t => t.Cuenta.FechaApertura)
            .Select(t => new TarjetaResponse(
                t.TarjetaId,
                t.NumeroTarjeta,
                t.NumeroTarjeta.Substring(12),
                t.Marca,
                t.Tipo,
                t.FechaExpiracion,
                t.Activa,
                t.CuentaId,
                t.Cuenta.NumeroCuenta,
                t.Cuenta.Saldo,
                t.Cuenta.Moneda))
            .ToListAsync(ct);
}
