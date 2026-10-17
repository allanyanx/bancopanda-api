using System.Security.Cryptography;
using BancoPanda.Api.Common;
using BancoPanda.Api.Data;
using BancoPanda.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BancoPanda.Api.Features.Transfers;

public class TransfersService(AppDbContext db)
{
    public async Task<Resultado<ComprobanteResponse>> TransferirAsync(
        Guid usuarioId, TransferenciaRequest request, CancellationToken ct)
    {
        var solicitudId = request.SolicitudId!.Value;
        var monto = request.Monto!.Value;

        var errorMonto = ReglasMonto.Validar(monto);
        if (errorMonto is not null) return Resultado<ComprobanteResponse>.Fallo(errorMonto);

        var repetida = await BuscarComprobanteAsync(usuarioId, solicitudId, ct);
        if (repetida is not null) return Resultado<ComprobanteResponse>.Ok(repetida);

        var origen = await db.Cuentas
            .Where(c => c.CuentaId == request.CuentaOrigenId && c.UsuarioId == usuarioId)
            .Select(c => new { c.CuentaId, c.NumeroCuenta, c.Usuario.Nombres, c.Usuario.Apellidos })
            .SingleOrDefaultAsync(ct);

        if (origen is null)
            return Resultado<ComprobanteResponse>.Fallo("Cuenta de origen no encontrada", StatusCodes.Status404NotFound);

        var banco = await db.Bancos
            .Where(b => b.BancoId == request.BancoId && b.Activo)
            .Select(b => new { b.BancoId, b.Nombre })
            .SingleOrDefaultAsync(ct);

        if (banco is null)
            return Resultado<ComprobanteResponse>.Fallo("Banco no encontrado", StatusCodes.Status404NotFound);

        var numeroCuenta = request.NumeroCuenta.Trim();
        var cedula = request.Cedula.Trim();
        var nombre = string.IsNullOrWhiteSpace(request.Nombre) ? null : request.Nombre.Trim();

        // Solo dentro de Banco Panda el dinero llega a una cuenta; a otros bancos se simula el envío.
        var destino = banco.BancoId == Banco.BancoPandaId
            ? await db.Cuentas
                .Where(c => c.NumeroCuenta == numeroCuenta && c.Usuario.Cedula == cedula)
                .Select(c => new { c.CuentaId, c.UsuarioId, c.Usuario.Nombres, c.Usuario.Apellidos })
                .SingleOrDefaultAsync(ct)
            : null;

        if (banco.BancoId == Banco.BancoPandaId)
        {
            if (destino is null)
                return Resultado<ComprobanteResponse>.Fallo(
                    "No encontramos una cuenta Banco Panda con ese número y cédula", StatusCodes.Status404NotFound);

            if (destino.CuentaId == origen.CuentaId)
                return Resultado<ComprobanteResponse>.Fallo("No puedes transferir a la misma cuenta");

            nombre = NombreCorto(destino.Nombres, destino.Apellidos);
        }

        await using var transaccion = await db.Database.BeginTransactionAsync(ct);

        var cuentas = await db.BloquearCuentasAsync(
            destino is null ? [origen.CuentaId] : [origen.CuentaId, destino.CuentaId], ct);
        var cuentaOrigen = cuentas[origen.CuentaId];

        if (cuentaOrigen.Saldo < monto)
            return Resultado<ComprobanteResponse>.Fallo("Saldo insuficiente para realizar la transferencia");

        var fecha = DateTime.UtcNow;
        var entreMisCuentas = destino?.UsuarioId == usuarioId;
        var paraQuien = nombre ?? $"cuenta {numeroCuenta}";

        cuentaOrigen.Saldo -= monto;
        var egreso = new Movimiento
        {
            CuentaId = origen.CuentaId,
            Descripcion = entreMisCuentas ? $"Transferencia a cuenta {numeroCuenta}"
                : destino is not null ? $"Transferencia a {paraQuien}"
                : $"Transferencia a {paraQuien} - {banco.Nombre}",
            Monto = monto,
            Tipo = TipoMovimiento.Egreso,
            Categoria = CategoriaMovimiento.Transferencia,
            Fecha = fecha
        };
        db.Movimientos.Add(egreso);

        if (destino is not null)
        {
            cuentas[destino.CuentaId].Saldo += monto;
            db.Movimientos.Add(new Movimiento
            {
                CuentaId = destino.CuentaId,
                Descripcion = entreMisCuentas
                    ? $"Transferencia desde cuenta {origen.NumeroCuenta}"
                    : $"Transferencia recibida de {NombreCorto(origen.Nombres, origen.Apellidos)}",
                Monto = monto,
                Tipo = TipoMovimiento.Ingreso,
                Categoria = CategoriaMovimiento.Transferencia,
                Fecha = fecha
            });
        }

        var beneficiario = await db.Beneficiarios.SingleOrDefaultAsync(
            b => b.UsuarioId == usuarioId && b.BancoId == banco.BancoId && b.NumeroCuenta == numeroCuenta, ct);

        if (beneficiario is null)
        {
            beneficiario = new Beneficiario { UsuarioId = usuarioId, BancoId = banco.BancoId, NumeroCuenta = numeroCuenta };
            db.Beneficiarios.Add(beneficiario);
        }
        beneficiario.Cedula = cedula;
        beneficiario.Nombre = nombre ?? beneficiario.Nombre;

        db.Transferencias.Add(new Transferencia
        {
            Referencia = await GenerarReferenciaAsync(fecha, ct),
            Monto = monto,
            Fecha = fecha,
            SolicitudId = solicitudId,
            CuentaOrigenId = origen.CuentaId,
            Beneficiario = beneficiario,
            Movimiento = egreso
        });

        try
        {
            await db.SaveChangesAsync(ct);
            await transaccion.CommitAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // Otra petición con la misma solicitud llegó primero: se devuelve su comprobante.
            await transaccion.RollbackAsync(ct);
            db.ChangeTracker.Clear();

            var previa = await BuscarComprobanteAsync(usuarioId, solicitudId, ct);
            return previa is not null
                ? Resultado<ComprobanteResponse>.Ok(previa)
                : Resultado<ComprobanteResponse>.Fallo(
                    "No se pudo completar la transferencia. Intenta de nuevo", StatusCodes.Status409Conflict);
        }

        return Resultado<ComprobanteResponse>.Ok((await BuscarComprobanteAsync(usuarioId, solicitudId, ct))!);
    }

    private Task<ComprobanteResponse?> BuscarComprobanteAsync(Guid usuarioId, Guid solicitudId, CancellationToken ct) =>
        db.Transferencias
            .Where(t => t.SolicitudId == solicitudId && t.CuentaOrigen.UsuarioId == usuarioId)
            .Select(t => new ComprobanteResponse(
                t.TransferenciaId,
                t.Referencia,
                t.Fecha,
                t.Monto,
                t.CuentaOrigen.Moneda,
                t.CuentaOrigen.NumeroCuenta,
                t.Beneficiario.Banco.Nombre,
                t.Beneficiario.NumeroCuenta,
                t.Beneficiario.Nombre,
                t.CuentaOrigen.Saldo))
            .SingleOrDefaultAsync(ct);

    private static string NombreCorto(string nombres, string apellidos) =>
        $"{nombres.Split(' ')[0]} {apellidos.Split(' ')[0]}";

    private async Task<string> GenerarReferenciaAsync(DateTime fecha, CancellationToken ct)
    {
        var dia = ReglasMonto.HoraEcuador(fecha).ToString("yyyyMMdd");
        while (true)
        {
            var referencia = dia + RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
            if (!await db.Transferencias.AnyAsync(t => t.Referencia == referencia, ct)) return referencia;
        }
    }
}
