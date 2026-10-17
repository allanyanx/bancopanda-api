namespace BancoPanda.Api.Entities;

public class Beneficiario
{
    public Guid BeneficiarioId { get; set; }
    public string NumeroCuenta { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public string? Nombre { get; set; }

    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public Guid BancoId { get; set; }
    public Banco Banco { get; set; } = null!;
}
