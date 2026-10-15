using System.ComponentModel.DataAnnotations;
using BancoPanda.Api.Entities;

namespace BancoPanda.Api.Features.Movements;

public class MovimientosRequest
{
    [Range(1, 50, ErrorMessage = "El límite debe estar entre 1 y 50")]
    public int Limite { get; set; } = 20;
}

public record MovimientoResponse(
    Guid MovimientoId,
    string Descripcion,
    decimal Monto,
    TipoMovimiento Tipo,
    CategoriaMovimiento Categoria,
    DateTime Fecha,
    Guid CuentaId);
