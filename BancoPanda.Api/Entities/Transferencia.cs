namespace BancoPanda.Api.Entities;

public class Transferencia
{
    public Guid TransferenciaId { get; set; }
    public string Referencia { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public Guid SolicitudId { get; set; }

    public Guid CuentaOrigenId { get; set; }
    public Cuenta CuentaOrigen { get; set; } = null!;
    public Guid BeneficiarioId { get; set; }
    public Beneficiario Beneficiario { get; set; } = null!;
    public Guid MovimientoId { get; set; }
    public Movimiento Movimiento { get; set; } = null!;
}
