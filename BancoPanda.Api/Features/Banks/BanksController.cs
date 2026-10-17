using BancoPanda.Api.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BancoPanda.Api.Features.Banks;

[ApiController]
[Authorize]
[Route("api/bancos")]
public class BanksController(BanksService banks) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<BancoResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken ct) =>
        Ok(await banks.ListarAsync(ct));
}
