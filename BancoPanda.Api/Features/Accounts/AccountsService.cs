using BancoPanda.Api.Common;
using BancoPanda.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace BancoPanda.Api.Features.Accounts;

public class AccountsService(AppDbContext db)
{
    private const int MovimientosRecientes = 5;
    private const string SinCuenta = "No tienes una cuenta registrada";

    public async Task<Resultado<ResumenResponse>> ObtenerResumenAsync(Guid usuarioId, CancellationToken ct)
    {
        var cuenta = await db.Cuentas
            .Where(c => c.UsuarioId == usuarioId)
            .OrderBy(c => c.FechaApertura)
            .Select(c => new CuentaResponse(c.CuentaId, c.NumeroCuenta, c.Tipo, c.Saldo, c.Moneda))
            .FirstOrDefaultAsync(ct);

        if (cuenta is null)
            return Resultado<ResumenResponse>.Fallo(SinCuenta, StatusCodes.Status404NotFound);

        var tarjetas = await db.Tarjetas
            .Where(t => t.CuentaId == cuenta.CuentaId && t.Activa)
            .OrderBy(t => t.FechaExpiracion)
            .Select(t => new TarjetaResponse(
                t.TarjetaId, t.CuentaId, t.UltimosDigitos, t.Marca, t.Tipo, t.FechaExpiracion))
            .ToListAsync(ct);

        var movimientos = await db.Movimientos
            .Where(m => m.CuentaId == cuenta.CuentaId)
            .OrderByDescending(m => m.Fecha)
            .Take(MovimientosRecientes)
            .Select(m => new MovimientoResponse(
                m.MovimientoId, m.CuentaId, m.Descripcion, m.Monto, m.Tipo, m.Categoria, m.Fecha))
            .ToListAsync(ct);

        return Resultado<ResumenResponse>.Ok(new ResumenResponse(cuenta, tarjetas, movimientos));
    }
}
