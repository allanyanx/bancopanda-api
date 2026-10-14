namespace BancoPanda.Api.Entities;

public class Usuario
{
    public Guid UsuarioId { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string ContrasenaHash { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public List<Cuenta> Cuentas { get; set; } = [];
}
