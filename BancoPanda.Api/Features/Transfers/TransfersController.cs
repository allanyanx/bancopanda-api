using BancoPanda.Api.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BancoPanda.Api.Features.Transfers;

[ApiController]
[Authorize]
[Route("api/transferencias")]
public class TransfersController(TransfersService transfers) : ApiControllerBase
{
    [HttpPost]
    [ProducesResponseType<ComprobanteResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Transferir(TransferenciaRequest request, CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var id)) return SesionInvalida();

        var resultado = await transfers.TransferirAsync(id, request, ct);

        return resultado.EsExitoso
            ? StatusCode(StatusCodes.Status201Created, resultado.Valor)
            : Problem(detail: resultado.Error, statusCode: resultado.StatusCode);
    }
}
