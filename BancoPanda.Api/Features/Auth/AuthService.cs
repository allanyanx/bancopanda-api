using System.Security.Cryptography;
using BancoPanda.Api.Common;
using BancoPanda.Api.Data;
using BancoPanda.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BancoPanda.Api.Features.Auth;

/// <summary>
/// Reglas de negocio del registro y del inicio de sesión.
/// Toda la lógica vive aquí: la app móvil solo muestra el resultado.
/// </summary>
public class AuthService(AppDbContext db, TokenService tokens)
{
    private const decimal BonoBienvenida = 100.00m;
    private const string CredencialesIncorrectas = "Correo, cédula o contraseña incorrectos";

    public async Task<Resultado<AuthResponse>> RegistrarAsync(RegistroRequest request, CancellationToken ct)
    {
        var correo = request.Correo.Trim().ToLowerInvariant();
        var cedula = request.Cedula.Trim();

        if (await db.Usuarios.AnyAsync(u => u.Correo == correo, ct))
            return Resultado<AuthResponse>.Fallo("Ya existe una cuenta con ese correo", StatusCodes.Status409Conflict);

        if (await db.Usuarios.AnyAsync(u => u.Cedula == cedula, ct))
            return Resultado<AuthResponse>.Fallo("Ya existe una cuenta con esa cédula", StatusCodes.Status409Conflict);

        var usuario = new Usuario
        {
            Nombres = request.Nombres.Trim(),
            Apellidos = request.Apellidos.Trim(),
            Cedula = cedula,
            Correo = correo,
            Telefono = request.Telefono.Trim(),
            ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(request.Contrasena)
        };

        // Todo usuario nuevo recibe una cuenta de ahorros, su tarjeta de débito
        // y un bono de bienvenida registrado como primer movimiento.
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
            UltimosDigitos = RandomNumberGenerator.GetInt32(0, 10_000).ToString("D4"),
            Marca = MarcaTarjeta.Visa,
            Tipo = TipoTarjeta.Debito,
            FechaExpiracion = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(4)
        });

        usuario.Cuentas.Add(cuenta);
        db.Usuarios.Add(usuario);

        try
        {
            // Un solo SaveChanges = una sola transacción: se guarda todo o nada.
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // Dos registros simultáneos con el mismo correo o cédula: gana el primero.
            return Resultado<AuthResponse>.Fallo("Ya existe una cuenta con esos datos", StatusCodes.Status409Conflict);
        }

        return CrearRespuesta(usuario);
    }

    public async Task<Resultado<AuthResponse>> IniciarSesionAsync(LoginRequest request, CancellationToken ct)
    {
        var identificador = request.Identificador.Trim();
        var correo = identificador.ToLowerInvariant();

        var usuario = identificador.Contains('@')
            ? await db.Usuarios.SingleOrDefaultAsync(u => u.Correo == correo, ct)
            : await db.Usuarios.SingleOrDefaultAsync(u => u.Cedula == identificador, ct);

        // Mismo mensaje si no existe el usuario o si la contraseña está mal:
        // así no se revela qué correos o cédulas están registrados.
        if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Contrasena, usuario.ContrasenaHash))
            return Resultado<AuthResponse>.Fallo(CredencialesIncorrectas, StatusCodes.Status401Unauthorized);

        return CrearRespuesta(usuario);
    }

    private Resultado<AuthResponse> CrearRespuesta(Usuario usuario)
    {
        var (token, expiraEn) = tokens.CrearToken(usuario);

        return Resultado<AuthResponse>.Ok(new AuthResponse(token, expiraEn, new UsuarioResponse(
            usuario.UsuarioId,
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
}
