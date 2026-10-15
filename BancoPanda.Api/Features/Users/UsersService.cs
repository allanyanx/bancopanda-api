using BancoPanda.Api.Common;
using BancoPanda.Api.Data;
using BancoPanda.Api.Entities;
using BancoPanda.Api.Features.Auth;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BancoPanda.Api.Features.Users;

public class UsersService(AppDbContext db)
{
    private const string NoEncontrado = "Usuario no encontrado";

    public async Task<Resultado<UsuarioResponse>> ObtenerAsync(Guid usuarioId, CancellationToken ct)
    {
        var usuario = await db.Usuarios.FindAsync([usuarioId], ct);

        return usuario is null
            ? Resultado<UsuarioResponse>.Fallo(NoEncontrado, StatusCodes.Status404NotFound)
            : Resultado<UsuarioResponse>.Ok(ToResponse(usuario));
    }

    public async Task<Resultado<UsuarioResponse>> ActualizarAsync(
        Guid usuarioId, ActualizarPerfilRequest request, CancellationToken ct)
    {
        var usuario = await db.Usuarios.FindAsync([usuarioId], ct);
        if (usuario is null)
            return Resultado<UsuarioResponse>.Fallo(NoEncontrado, StatusCodes.Status404NotFound);

        var nombreUsuario = request.NombreUsuario.Trim().ToLowerInvariant();
        var correo = request.Correo.Trim().ToLowerInvariant();

        if (await db.Usuarios.AnyAsync(u => u.NombreUsuario == nombreUsuario && u.UsuarioId != usuarioId, ct))
            return Resultado<UsuarioResponse>.Fallo("Ese nombre de usuario ya está en uso", StatusCodes.Status409Conflict);

        if (await db.Usuarios.AnyAsync(u => u.Correo == correo && u.UsuarioId != usuarioId, ct))
            return Resultado<UsuarioResponse>.Fallo("Ya existe una cuenta con ese correo", StatusCodes.Status409Conflict);

        usuario.NombreUsuario = nombreUsuario;
        usuario.Nombres = request.Nombres.Trim();
        usuario.Apellidos = request.Apellidos.Trim();
        usuario.Correo = correo;
        usuario.Telefono = request.Telefono.Trim();

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            return Resultado<UsuarioResponse>.Fallo("Ya existe una cuenta con esos datos", StatusCodes.Status409Conflict);
        }

        return Resultado<UsuarioResponse>.Ok(ToResponse(usuario));
    }

    public async Task<Resultado<bool>> CambiarContrasenaAsync(
        Guid usuarioId, CambiarContrasenaRequest request, CancellationToken ct)
    {
        var usuario = await db.Usuarios.FindAsync([usuarioId], ct);
        if (usuario is null)
            return Resultado<bool>.Fallo(NoEncontrado, StatusCodes.Status404NotFound);

        // 400 y no 401: la app cierra la sesión ante un 401 y aquí el token es válido.
        if (!BCrypt.Net.BCrypt.Verify(request.ContrasenaActual, usuario.ContrasenaHash))
            return Resultado<bool>.Fallo("La contraseña actual es incorrecta");

        if (request.ContrasenaActual == request.ContrasenaNueva)
            return Resultado<bool>.Fallo("La contraseña nueva debe ser distinta a la actual");

        usuario.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(request.ContrasenaNueva);
        await db.SaveChangesAsync(ct);

        return Resultado<bool>.Ok(true);
    }

    private static UsuarioResponse ToResponse(Usuario u) =>
        new(u.UsuarioId, u.NombreUsuario, u.Nombres, u.Apellidos, u.Cedula, u.Correo, u.Telefono);
}