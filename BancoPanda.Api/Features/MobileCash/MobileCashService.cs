using System.Security.Cryptography;
using BancoPanda.Api.Common;
using BancoPanda.Api.Data;
using BancoPanda.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BancoPanda.Api.Features.MobileCash;

public class MobileCashService(AppDbContext db)
{
    public const int HorasVigencia = 24;
    private const string YaGenerado = "Este efectivo móvil ya se generó. Revisa tus movimientos";

    public async Task<Resultado<EfectivoMovilResponse>> CrearAsync(
        Guid usuarioId, EfectivoMovilRequest request, CancellationToken ct)
    {
        var solicitudId = request.SolicitudId!.Value;
        var monto = request.Monto!.Value;

        var errorMonto = ReglasMonto.Validar(monto);
        if (errorMonto is not null) return Resultado<EfectivoMovilResponse>.Fallo(errorMonto);

        // El código solo se guarda cifrado, así que no se puede volver a mostrar.
        if (await db.EfectivosMoviles.AnyAsync(e => e.SolicitudId == solicitudId, ct))
            return Resultado<EfectivoMovilResponse>.Fallo(YaGenerado, StatusCodes.Status409Conflict);

        var cuentaId = await db.Cuentas
            .Where(c => c.CuentaId == request.CuentaId && c.UsuarioId == usuarioId)
            .Select(c => (Guid?)c.CuentaId)
            .SingleOrDefaultAsync(ct);

        if (cuentaId is null)
            return Resultado<EfectivoMovilResponse>.Fallo("Cuenta no encontrada", StatusCodes.Status404NotFound);

        await using var transaccion = await db.Database.BeginTransactionAsync(ct);

        var cuenta = (await db.BloquearCuentasAsync([cuentaId.Value], ct))[cuentaId.Value];

        if (cuenta.Saldo < monto)
            return Resultado<EfectivoMovilResponse>.Fallo("Saldo insuficiente para generar el efectivo móvil");

        var telefono = request.TelefonoDestino.Trim();
        var codigo = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        var fecha = DateTime.UtcNow;

        cuenta.Saldo -= monto;

        var efectivo = new EfectivoMovil
        {
            TelefonoDestino = telefono,
            Monto = monto,
            CodigoHash = BCrypt.Net.BCrypt.HashPassword(codigo),
            FechaCreacion = fecha,
            FechaExpiracion = fecha.AddHours(HorasVigencia),
            SolicitudId = solicitudId,
            CuentaId = cuenta.CuentaId,
            Movimiento = new Movimiento
            {
                CuentaId = cuenta.CuentaId,
                Descripcion = $"Efectivo móvil para {telefono}",
                Monto = monto,
                Tipo = TipoMovimiento.Egreso,
                Categoria = CategoriaMovimiento.EfectivoMovil,
                Fecha = fecha
            }
        };
        db.EfectivosMoviles.Add(efectivo);

        try
        {
            await db.SaveChangesAsync(ct);
            await transaccion.CommitAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            return Resultado<EfectivoMovilResponse>.Fallo(YaGenerado, StatusCodes.Status409Conflict);
        }

        return Resultado<EfectivoMovilResponse>.Ok(new EfectivoMovilResponse(
            efectivo.EfectivoMovilId,
            codigo,
            efectivo.Monto,
            cuenta.Moneda,
            efectivo.TelefonoDestino,
            efectivo.FechaCreacion,
            efectivo.FechaExpiracion,
            efectivo.Estado,
            cuenta.NumeroCuenta,
            cuenta.Saldo));
    }
}
