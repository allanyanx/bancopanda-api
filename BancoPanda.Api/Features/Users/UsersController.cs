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
        => Ok(await users.ObtenerAsync(UsuarioId(), ct));

    [HttpPut("me")]
    [ProducesResponseType<UsuarioResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Actualizar(ActualizarPerfilRequest request, CancellationToken ct)
        => Ok(await users.ActualizarAsync(UsuarioId(), request, ct));

    // El id sale del token 
    private Guid UsuarioId()
    {
        var valor = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return Guid.TryParse(valor, out var id)
            ? id
            : throw new ReglaNegocioException("Sesión no válida", StatusCodes.Status401Unauthorized);
    }
}