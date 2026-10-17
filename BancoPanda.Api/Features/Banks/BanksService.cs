using BancoPanda.Api.Data;
using BancoPanda.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace BancoPanda.Api.Features.Banks;

public class BanksService(AppDbContext db)
{
    public Task<List<BancoResponse>> ListarAsync(CancellationToken ct) =>
        db.Bancos
            .Where(b => b.Activo)
            .OrderBy(b => b.BancoId != Banco.BancoPandaId)
            .ThenBy(b => b.Nombre)
            .Select(b => new BancoResponse(b.BancoId, b.Nombre))
            .ToListAsync(ct);
}
