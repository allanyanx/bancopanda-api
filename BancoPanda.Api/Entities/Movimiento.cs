namespace BancoPanda.Api.Entities;

public class Movimiento
{
    public Guid MovimientoId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public TipoMovimiento Tipo { get; set; }
    public CategoriaMovimiento Categoria { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public Guid CuentaId { get; set; }
    public Cuenta Cuenta { get; set; } = null!;
}