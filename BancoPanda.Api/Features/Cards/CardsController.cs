using BancoPanda.Api.Common;
using BancoPanda.Api.Features.Movements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BancoPanda.Api.Features.Cards;

[ApiController]
[Authorize]
[Route("api/tarjetas")]
public class CardsController(CardsService cards, MovementsService movements) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<TarjetaResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var id)) return SesionInvalida();

        return Ok(await cards.ListarAsync(id, ct));
    }

    [HttpGet("{tarjetaId:guid}/movimientos")]
    [ProducesResponseType<List<MovimientoResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListarMovimientos(
        Guid tarjetaId, [FromQuery] MovimientosRequest request, CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var id)) return SesionInvalida();

        return ARespuesta(await movements.ListarPorTarjetaAsync(id, tarjetaId, request.Limite, ct));
    }
}
