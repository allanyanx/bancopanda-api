using BancoPanda.Api.Common;
using BancoPanda.Api.Data;
using BancoPanda.Api.Entities;
using BancoPanda.Api.Features.Auth;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BancoPanda.Api.Features.Users;

public class UsersService(AppDbContext db)
{
    private const string CorreoRepetido = "Ya existe una cuenta con ese correo";

    public async Task<Resultado<UsuarioResponse>> ObtenerAsync(Guid usuarioId, CancellationToken ct)
    {
        var usuario = await db.Usuarios.FindAsync([usuarioId], ct);

        return usuario is null
            ? Resultado<UsuarioResponse>.Fallo("Usuario no encontrado", StatusCodes.Status404NotFound)
            : Resultado<UsuarioResponse>.Ok(ToResponse(usuario));
    }

    public async Task<Resultado<UsuarioResponse>> ActualizarAsync(
        Guid usuarioId, ActualizarPerfilRequest request, CancellationToken ct)
    {
        var usuario = await db.Usuarios.FindAsync([usuarioId], ct);
        if (usuario is null)
            return Resultado<UsuarioResponse>.Fallo("Usuario no encontrado", StatusCodes.Status404NotFound);

        var correo = request.Correo.Trim().ToLowerInvariant();

        if (await db.Usuarios.AnyAsync(u => u.Correo == correo && u.UsuarioId != usuarioId, ct))
            return Resultado<UsuarioResponse>.Fallo(CorreoRepetido, StatusCodes.Status409Conflict);

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
            return Resultado<UsuarioResponse>.Fallo(CorreoRepetido, StatusCodes.Status409Conflict);
        }

        return Resultado<UsuarioResponse>.Ok(ToResponse(usuario));
    }

    private static UsuarioResponse ToResponse(Usuario u) =>
        new(u.UsuarioId, u.Nombres, u.Apellidos, u.Cedula, u.Correo, u.Telefono);
}