namespace BancoPanda.Api.Common;

public static class ReglasValidacion
{
    public const string SoloLetras = @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ ]+$";

    public const string NombreUsuario = @"^[a-zA-Z][a-zA-Z0-9._]{3,19}$";
    public const string MensajeNombreUsuario =
        "El nombre de usuario debe tener entre 4 y 20 caracteres, empezar con una letra y usar solo letras, números, punto (.) o guion bajo (_)";

    public const string Correo = @"^(?!.*\.\.)[a-zA-Z0-9]([a-zA-Z0-9._-]*[a-zA-Z0-9])?@[a-zA-Z0-9]([a-zA-Z0-9-]*[a-zA-Z0-9])?(\.[a-zA-Z0-9]([a-zA-Z0-9-]*[a-zA-Z0-9])?)*\.[a-zA-Z]{2,}$";
    public const string MensajeCorreo =
        "Escribe un correo válido, como nombre@dominio.com. Solo se permiten letras, números, punto (.), guion (-) y guion bajo (_)";

    public const string Celular = @"^\+5939\d{8}$";
    public const string MensajeCelular =
        "El celular debe tener el código de Ecuador (+593) seguido de 9 dígitos que empiecen con 9";
}
