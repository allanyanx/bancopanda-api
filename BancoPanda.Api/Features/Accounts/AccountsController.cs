using BancoPanda.Api.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BancoPanda.Api.Features.Accounts;

[ApiController]
[Authorize]
[Route("api/cuentas")]
public class AccountsController(AccountsService accounts) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<CuentaResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var id)) return SesionInvalida();

        return Ok(await accounts.ListarAsync(id, ct));
    }
}
