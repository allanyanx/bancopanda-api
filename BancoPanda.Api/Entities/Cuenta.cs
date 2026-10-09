namespace BancoPanda.Api.Entities;

public class Cuenta
{
    public Guid Id { get; set; }
    public string NumeroCuenta { get; set; } = string.Empty;
    public TipoCuenta Tipo { get; set; }
    public decimal Saldo { get; set; }
    public string Moneda { get; set; } = "USD";
    public DateTime FechaApertura { get; set; } = DateTime.UtcNow;

    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public List<Tarjeta> Tarjetas { get; set; } = [];
    public List<Movimiento> Movimientos { get; set; } = [];
}