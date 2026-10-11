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
    [HttpGet("perfil")]
    [ProducesResponseType<UsuarioResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obtener(CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var id)) return SesionInvalida();

        return ARespuesta(await users.ObtenerAsync(id, ct));
    }

    [HttpPut("perfil")]
    [ProducesResponseType<UsuarioResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Actualizar(ActualizarPerfilRequest request, CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var id)) return SesionInvalida();

        return ARespuesta(await users.ActualizarAsync(id, request, ct));
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