using System.ComponentModel.DataAnnotations;
using BancoPanda.Api.Common;

namespace BancoPanda.Api.Features.Users;

public class ActualizarPerfilRequest
{
    [Required(ErrorMessage = "Los nombres son obligatorios")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Los nombres deben tener entre 2 y 60 caracteres")]
    [RegularExpression(ReglasValidacion.SoloLetras, ErrorMessage = "Los nombres solo pueden tener letras")]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos son obligatorios")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "Los apellidos deben tener entre 2 y 60 caracteres")]
    [RegularExpression(ReglasValidacion.SoloLetras, ErrorMessage = "Los apellidos solo pueden tener letras")]
    public string Apellidos { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "El correo no es válido")]
    [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [RegularExpression(@"^09\d{8}$", ErrorMessage = "El teléfono debe tener 10 dígitos y empezar con 09")]
    public string Telefono { get; set; } = string.Empty;

    public string? ContrasenaActual { get; set; }

    [StringLength(72, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 72 caracteres")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$", ErrorMessage = "La contraseña debe incluir letras y números")]
    public string? ContrasenaNueva { get; set; }
}
