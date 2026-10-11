using BancoPanda.Api.Common;
using BancoPanda.Api.Data;
using BancoPanda.Api.Entities;
using BancoPanda.Api.Features.Auth;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BancoPanda.Api.Features.Users;

public class UsersService(AppDbContext db)
{
    public async Task<UsuarioResponse> ObtenerAsync(Guid usuarioId, CancellationToken ct)
        => ToResponse(await BuscarAsync(usuarioId, ct));

    public async Task<UsuarioResponse> ActualizarAsync(
        Guid usuarioId, ActualizarPerfilRequest request, CancellationToken ct)
    {
        var usuario = await BuscarAsync(usuarioId, ct);
        var correo = request.Correo.Trim().ToLowerInvariant();

        if (await db.Usuarios.AnyAsync(u => u.Correo == correo && u.UsuarioId != usuarioId, ct))
            throw new ReglaNegocioException("Ya existe una cuenta con ese correo", StatusCodes.Status409Conflict);

        usuario.Nombres = request.Nombres.Trim();
        usuario.Apellidos = request.Apellidos.Trim();
        usuario.Correo = correo;
        usuario.Telefono = request.Telefono.Trim();

        if (!string.IsNullOrEmpty(request.ContrasenaNueva))
            usuario.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(request.ContrasenaNueva);

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new ReglaNegocioException("Ya existe una cuenta con ese correo", StatusCodes.Status409Conflict);
        }

        return ToResponse(usuario);
    }

    private async Task<Usuario> BuscarAsync(Guid usuarioId, CancellationToken ct)
        => await db.Usuarios.FindAsync([usuarioId], ct)
           ?? throw new ReglaNegocioException("Usuario no encontrado", StatusCodes.Status404NotFound);

    private static UsuarioResponse ToResponse(Usuario u) =>
        new(u.UsuarioId, u.Nombres, u.Apellidos, u.Cedula, u.Correo, u.Telefono);
}