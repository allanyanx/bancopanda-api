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
        var resultado = await auth.RegistrarAsync(request, ct);

        // Problem() responde en formato ProblemDetails: la app lee el mensaje en "detail".
        return resultado.EsExitoso
            ? StatusCode(StatusCodes.Status201Created, resultado.Valor)
            : Problem(detail: resultado.Error, statusCode: resultado.StatusCode);
    }

    /// <summary>Inicia sesión con correo o cédula.</summary>
    [HttpPost("login")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> IniciarSesion(LoginRequest request, CancellationToken ct)
    {
        var resultado = await auth.IniciarSesionAsync(request, ct);

        return resultado.EsExitoso
            ? Ok(resultado.Valor)
            : Problem(detail: resultado.Error, statusCode: resultado.StatusCode);
    }
}
