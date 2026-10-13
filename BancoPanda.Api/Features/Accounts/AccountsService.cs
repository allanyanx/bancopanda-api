using BancoPanda.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace BancoPanda.Api.Features.Accounts;

public class AccountsService(AppDbContext db)
{
    public Task<List<CuentaResponse>> ListarAsync(Guid usuarioId, CancellationToken ct) =>
        db.Cuentas
            .Where(c => c.UsuarioId == usuarioId)
            .OrderBy(c => c.FechaApertura)
            .Select(c => new CuentaResponse(c.CuentaId, c.NumeroCuenta, c.Tipo, c.Saldo, c.Moneda))
            .ToListAsync(ct);
}
