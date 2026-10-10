namespace BancoPanda.Api.Common;

/// <summary>
/// Error de negocio con un mensaje listo para mostrar al usuario
/// (correo repetido, credenciales incorrectas, etc.).
/// </summary>
public class ReglaNegocioException(string mensaje, int statusCode = StatusCodes.Status400BadRequest)
    : Exception(mensaje)
{
    public int StatusCode { get; } = statusCode;
}
