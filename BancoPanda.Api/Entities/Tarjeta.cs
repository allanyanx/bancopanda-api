namespace BancoPanda.Api.Entities;

public class Tarjeta
{
    public Guid Id { get; set; }
    public string UltimosDigitos { get; set; } = string.Empty;   
    public MarcaTarjeta Marca { get; set; }
    public TipoTarjeta Tipo { get; set; }
    public DateOnly FechaExpiracion { get; set; }
    public bool Activa { get; set; } = true;

    public Guid CuentaId { get; set; }
    public Cuenta Cuenta { get; set; } = null!;
}