using BancoPanda.Api.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BancoPanda.Api.Features.MobileCash;

[ApiController]
[Authorize]
[Route("api/efectivo-movil")]
public class MobileCashController(MobileCashService mobileCash) : ApiControllerBase
{
    [HttpPost]
    [ProducesResponseType<EfectivoMovilResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Crear(EfectivoMovilRequest request, CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var id)) return SesionInvalida();

        var resultado = await mobileCash.CrearAsync(id, request, ct);

        return resultado.EsExitoso
            ? StatusCode(StatusCodes.Status201Created, resultado.Valor)
            : Problem(detail: resultado.Error, statusCode: resultado.StatusCode);
    }
}
