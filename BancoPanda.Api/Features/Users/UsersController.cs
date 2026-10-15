using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BancoPanda.Api.Common;
using BancoPanda.Api.Features.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BancoPanda.Api.Features.Users;

[ApiController]
[Authorize]
[Route("api/usuarios")]
public class UsersController(UsersService users) : ControllerBase
{
    [HttpGet("me")]
    [ProducesResponseType<UsuarioResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Obtener(CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var id)) return SesionInvalida();
        return ARespuesta(await users.ObtenerAsync(id, ct));
    }

    [HttpPut("me")]
    [ProducesResponseType<UsuarioResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Actualizar(ActualizarPerfilRequest request, CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var id)) return SesionInvalida();
        return ARespuesta(await users.ActualizarAsync(id, request, ct));
    }

    [HttpPut("me/contrasena")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CambiarContrasena(CambiarContrasenaRequest request, CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var id)) return SesionInvalida();

        var resultado = await users.CambiarContrasenaAsync(id, request, ct);
        return resultado.EsExitoso
            ? NoContent()
            : Problem(detail: resultado.Error, statusCode: resultado.StatusCode);
    }

    private IActionResult ARespuesta(Resultado<UsuarioResponse> resultado) =>
        resultado.EsExitoso
            ? Ok(resultado.Valor)
            : Problem(detail: resultado.Error, statusCode: resultado.StatusCode);

    private IActionResult SesionInvalida() =>
        Problem(detail: "Sesión no válida", statusCode: StatusCodes.Status401Unauthorized);

    private bool TryGetUsuarioId(out Guid id) =>
        Guid.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub), out id);
}