using BancoPanda.Api.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BancoPanda.Api.Features.Beneficiaries;

[ApiController]
[Authorize]
[Route("api/beneficiarios")]
public class BeneficiariesController(BeneficiariesService beneficiaries) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<BeneficiarioResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var id)) return SesionInvalida();

        return Ok(await beneficiaries.ListarAsync(id, ct));
    }
}
