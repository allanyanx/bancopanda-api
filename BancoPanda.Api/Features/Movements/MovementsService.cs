using BancoPanda.Api.Common;
using BancoPanda.Api.Data;
using BancoPanda.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace BancoPanda.Api.Features.Movements;

public class MovementsService(AppDbContext db)
{
    public Task<List<MovimientoResponse>> ListarRecientesAsync(Guid usuarioId, int limite, CancellationToken ct) =>
        MasRecientesAsync(db.Movimientos.Where(m => m.Cuenta.UsuarioId == usuarioId), limite, ct);

    public async Task<Resultado<List<MovimientoResponse>>> ListarPorTarjetaAsync(
        Guid usuarioId, Guid tarjetaId, int limite, CancellationToken ct)
    {
        var cuentaId = await db.Tarjetas
            .Where(t => t.TarjetaId == tarjetaId && t.Cuenta.UsuarioId == usuarioId)
            .Select(t => (Guid?)t.CuentaId)
            .SingleOrDefaultAsync(ct);

        if (cuentaId is null)
            return Resultado<List<MovimientoResponse>>.Fallo("Tarjeta no encontrada", StatusCodes.Status404NotFound);

        return Resultado<List<MovimientoResponse>>.Ok(
            await MasRecientesAsync(db.Movimientos.Where(m => m.CuentaId == cuentaId), limite, ct));
    }

    private static Task<List<MovimientoResponse>> MasRecientesAsync(
        IQueryable<Movimiento> movimientos, int limite, CancellationToken ct) =>
        movimientos
            .OrderByDescending(m => m.Fecha)
            .Take(limite)
            .Select(m => new MovimientoResponse(
                m.MovimientoId, m.Descripcion, m.Monto, m.Tipo, m.Categoria, m.Fecha, m.CuentaId))
            .ToListAsync(ct);
}
