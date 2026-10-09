namespace BancoPanda.Api.Entities;

public class Movimiento
{
    public Guid Id { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal Monto { get; set; }                    // Siempre positivo
    public TipoMovimiento Tipo { get; set; }              // Ingreso suma, Egreso resta
    public CategoriaMovimiento Categoria { get; set; }    // Define el ícono en la app
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public Guid CuentaId { get; set; }
    public Cuenta Cuenta { get; set; } = null!;
}