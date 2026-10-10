using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BancoPanda.Api.Common;

/// <summary>
/// Convierte las ReglaNegocioException en una respuesta JSON estándar (ProblemDetails).
/// La app lee el campo "detail" para mostrar el mensaje.
/// </summary>
public class ManejadorErrores(IProblemDetailsService problemDetails) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ReglaNegocioException regla) return false;

        httpContext.Response.StatusCode = regla.StatusCode;

        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = regla.StatusCode,
                Detail = regla.Message
            }
        });
    }
}
