namespace BancoPanda.Api.Features.Auth;

/// <summary>Configuración del token. La clave se guarda en User Secrets, nunca en el repositorio.</summary>
public class JwtOptions
{
    public const string Seccion = "Jwt";

    public string Clave { get; set; } = string.Empty;
    public string Emisor { get; set; } = string.Empty;
    public string Audiencia { get; set; } = string.Empty;
    public int MinutosExpiracion { get; set; } = 60;
}
