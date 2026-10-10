using Microsoft.AspNetCore.Mvc;

namespace BancoPanda.Api.Features.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthService auth) : ControllerBase
{
    /// <summary>Crea el usuario con su cuenta y devuelve la sesión iniciada.</summary>
    [HttpPost("registro")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Registrar(RegistroRequest request, CancellationToken ct)
    {
        var respuesta = await auth.RegistrarAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, respuesta);
    }

    /// <summary>Inicia sesión con correo o cédula.</summary>
    [HttpPost("login")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> IniciarSesion(LoginRequest request, CancellationToken ct)
    {
        var respuesta = await auth.IniciarSesionAsync(request, ct);
        return Ok(respuesta);
    }
}
