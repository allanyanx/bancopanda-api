namespace BancoPanda.Api.Common;

public class Resultado<T>
{
    private Resultado(T? valor, string? error, int statusCode)
    {
        Valor = valor;
        Error = error;
        StatusCode = statusCode;
    }

    public T? Valor { get; }
    public string? Error { get; }
    public int StatusCode { get; }
    public bool EsExitoso => Error is null;

    public static Resultado<T> Ok(T valor) => new(valor, null, StatusCodes.Status200OK);

    public static Resultado<T> Fallo(string error, int statusCode = StatusCodes.Status400BadRequest) =>
        new(default, error, statusCode);
}
