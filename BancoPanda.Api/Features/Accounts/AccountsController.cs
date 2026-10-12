using BancoPanda.Api.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BancoPanda.Api.Features.Accounts;

[ApiController]
[Authorize]
[Route("api/cuentas")]
public class AccountsController(AccountsService accounts) : ApiControllerBase
{
    [HttpGet("resumen")]
    [ProducesResponseType<ResumenResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerResumen(CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var id)) return SesionInvalida();

        return ARespuesta(await accounts.ObtenerResumenAsync(id, ct));
    }
}
