namespace BancoPanda.Api.Entities;

public class EfectivoMovil
{
    public Guid EfectivoMovilId { get; set; }
    public string TelefonoDestino { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string CodigoHash { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaExpiracion { get; set; }
    public EstadoEfectivoMovil Estado { get; set; } = EstadoEfectivoMovil.Pendiente;
    public Guid SolicitudId { get; set; }

    public Guid CuentaId { get; set; }
    public Cuenta Cuenta { get; set; } = null!;
    public Guid MovimientoId { get; set; }
    public Movimiento Movimiento { get; set; } = null!;
}
