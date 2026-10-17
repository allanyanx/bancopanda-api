using System.Security.Cryptography;
using BancoPanda.Api.Common;
using BancoPanda.Api.Data;
using BancoPanda.Api.Entities;
using BancoPanda.Api.Features.Cards;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BancoPanda.Api.Features.Auth;

public class AuthService(AppDbContext db, TokenService tokens)
{
    private const decimal BonoBienvenida = 100.00m;
    private const string CredencialesIncorrectas = "Usuario o contraseña incorrectos";

    public async Task<Resultado<AuthResponse>> RegistrarAsync(RegistroRequest request, CancellationToken ct)
    {
        var nombreUsuario = request.NombreUsuario.Trim().ToLowerInvariant();
        var correo = request.Correo.Trim().ToLowerInvariant();
        var cedula = request.Cedula.Trim();

        if (await db.Usuarios.AnyAsync(u => u.NombreUsuario == nombreUsuario, ct))
            return Resultado<AuthResponse>.Fallo("Ese nombre de usuario ya está en uso", StatusCodes.Status409Conflict);

        if (await db.Usuarios.AnyAsync(u => u.Correo == correo, ct))
            return Resultado<AuthResponse>.Fallo("Ya existe una cuenta con ese correo", StatusCodes.Status409Conflict);

        if (await db.Usuarios.AnyAsync(u => u.Cedula == cedula, ct))
            return Resultado<AuthResponse>.Fallo("Ya existe una cuenta con esa cédula", StatusCodes.Status409Conflict);

        var usuario = new Usuario
        {
            NombreUsuario = nombreUsuario,
            Nombres = request.Nombres.Trim(),
            Apellidos = request.Apellidos.Trim(),
            Cedula = cedula,
            Correo = correo,
            Telefono = request.Telefono.Trim(),
            ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(request.Contrasena)
        };

        var cuenta = new Cuenta
        {
            NumeroCuenta = await GenerarNumeroCuentaAsync(ct),
            Tipo = TipoCuenta.Ahorros,
            Saldo = BonoBienvenida
        };

        cuenta.Movimientos.Add(new Movimiento
        {
            Descripcion = "Bono de bienvenida",
            Monto = BonoBienvenida,
            Tipo = TipoMovimiento.Ingreso,
            Categoria = CategoriaMovimiento.Otros
        });

        cuenta.Tarjetas.Add(new Tarjeta
        {
            NumeroTarjeta = await GenerarNumeroTarjetaAsync(ct),
            Marca = MarcaTarjeta.Visa,
            Tipo = TipoTarjeta.Debito,
            FechaExpiracion = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(4)
        });

        usuario.Cuentas.Add(cuenta);
        db.Usuarios.Add(usuario);

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            return Resultado<AuthResponse>.Fallo("Ya existe una cuenta con esos datos", StatusCodes.Status409Conflict);
        }

        return CrearRespuesta(usuario);
    }

    public async Task<Resultado<AuthResponse>> IniciarSesionAsync(LoginRequest request, CancellationToken ct)
    {
        var nombreUsuario = request.NombreUsuario.Trim().ToLowerInvariant();

        var usuario = await db.Usuarios.SingleOrDefaultAsync(u => u.NombreUsuario == nombreUsuario, ct);

        if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Contrasena, usuario.ContrasenaHash))
            return Resultado<AuthResponse>.Fallo(CredencialesIncorrectas, StatusCodes.Status401Unauthorized);

        return CrearRespuesta(usuario);
    }

    private Resultado<AuthResponse> CrearRespuesta(Usuario usuario)
    {
        var (token, expiraEn) = tokens.CrearToken(usuario);

        return Resultado<AuthResponse>.Ok(new AuthResponse(token, expiraEn, new UsuarioResponse(
            usuario.UsuarioId,
            usuario.NombreUsuario,
            usuario.Nombres,
            usuario.Apellidos,
            usuario.Cedula,
            usuario.Correo,
            usuario.Telefono)));
    }

    private async Task<string> GenerarNumeroCuentaAsync(CancellationToken ct)
    {
        while (true)
        {
            var numero = "5000" + RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
            if (!await db.Cuentas.AnyAsync(c => c.NumeroCuenta == numero, ct)) return numero;
        }
    }

    private async Task<string> GenerarNumeroTarjetaAsync(CancellationToken ct)
    {
        while (true)
        {
            var numero = GeneradorTarjeta.Generar(MarcaTarjeta.Visa);
            if (!await db.Tarjetas.AnyAsync(t => t.NumeroTarjeta == numero, ct)) return numero;
        }
    }
}
