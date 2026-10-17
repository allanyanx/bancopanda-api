namespace BancoPanda.Api.Entities;

public class Banco
{
    public static readonly Guid BancoPandaId = new("b0000000-0000-4000-8000-000000000001");

    public Guid BancoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
