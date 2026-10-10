using System.ComponentModel.DataAnnotations;

namespace BancoPanda.Api.Features.Auth;

// Los nombres de los campos son los mismos que en las entidades y en la app
// (en JSON viajan en camelCase: usuarioId, nombres, apellidos...).

public class RegistroRequest
{
    [Required(ErrorMessage = "La cédula es obligatoria")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "La cédula debe tener 10 dígitos")]
    public string Cedula { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los nombres son obligatorios")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Los nombres deben tener entre 2 y 60 caracteres")]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Los apellidos deben tener entre 2 y 60 caracteres")]
    public string Apellidos { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "El correo no es válido")]
    [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [RegularExpression(@"^09\d{8}$", ErrorMessage = "El teléfono debe tener 10 dígitos y empezar con 09")]
    public string Telefono { get; set; } = string.Empty;

    // BCrypt solo usa los primeros 72 bytes, por eso el máximo.
    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [StringLength(72, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 72 caracteres")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$", ErrorMessage = "La contraseña debe incluir letras y números")]
    public string Contrasena { get; set; } = string.Empty;
}

public class LoginRequest
{
    /// <summary>Correo electrónico o cédula.</summary>
    [Required(ErrorMessage = "Ingresa tu correo o cédula")]
    public string Identificador { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa tu contraseña")]
    public string Contrasena { get; set; } = string.Empty;
}

public record UsuarioResponse(
    Guid UsuarioId,
    string Nombres,
    string Apellidos,
    string Cedula,
    string Correo,
    string? Telefono);

public record AuthResponse(string Token, DateTime ExpiraEn, UsuarioResponse Usuario);
