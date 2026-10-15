using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace BancoPanda.Api.Common;

public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult ARespuesta<T>(Resultado<T> resultado) =>
        resultado.EsExitoso
            ? Ok(resultado.Valor)
            : Problem(detail: resultado.Error, statusCode: resultado.StatusCode);

    protected IActionResult SesionInvalida() =>
        Problem(detail: "Sesión no válida", statusCode: StatusCodes.Status401Unauthorized);

    protected bool TryGetUsuarioId(out Guid id) =>
        Guid.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub), out id);
}
