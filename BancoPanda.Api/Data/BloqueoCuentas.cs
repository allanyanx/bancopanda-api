using BancoPanda.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace BancoPanda.Api.Data;

public static class BloqueoCuentas
{
    // Bloquea las cuentas (siempre en el mismo orden) hasta que termine la transacción,
    // para que dos operaciones simultáneas no lean el mismo saldo.
    public static Task<Dictionary<Guid, Cuenta>> BloquearCuentasAsync(
        this AppDbContext db, IEnumerable<Guid> cuentaIds, CancellationToken ct)
    {
        Guid[] ids = [.. cuentaIds.Distinct().Order()];
        return db.Cuentas
            .FromSql($"SELECT * FROM cuentas WHERE cuenta_id = ANY({ids}) ORDER BY cuenta_id FOR UPDATE")
            .ToDictionaryAsync(c => c.CuentaId, ct);
    }
}
