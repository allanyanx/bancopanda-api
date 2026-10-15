using BancoPanda.Api.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BancoPanda.Api.Features.Movements;

[ApiController]
[Authorize]
[Route("api/movimientos")]
public class MovementsController(MovementsService movements) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<MovimientoResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListarRecientes([FromQuery] MovimientosRequest request, CancellationToken ct)
    {
        if (!TryGetUsuarioId(out var id)) return SesionInvalida();

        return Ok(await movements.ListarRecientesAsync(id, request.Limite, ct));
    }
}
