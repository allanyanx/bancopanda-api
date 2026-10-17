using BancoPanda.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace BancoPanda.Api.Features.Beneficiaries;

public class BeneficiariesService(AppDbContext db)
{
    // Primero a quien se le transfirió más recientemente.
    public Task<List<BeneficiarioResponse>> ListarAsync(Guid usuarioId, CancellationToken ct) =>
        db.Beneficiarios
            .Where(b => b.UsuarioId == usuarioId)
            .OrderByDescending(b => db.Transferencias
                .Where(t => t.BeneficiarioId == b.BeneficiarioId)
                .Max(t => (DateTime?)t.Fecha) ?? DateTime.MinValue)
            .ThenBy(b => b.Nombre)
            .Select(b => new BeneficiarioResponse(
                b.BeneficiarioId, b.BancoId, b.Banco.Nombre, b.NumeroCuenta, b.Cedula, b.Nombre))
            .ToListAsync(ct);
}
