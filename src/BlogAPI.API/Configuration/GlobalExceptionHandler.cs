using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.API.Configuration;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "Ocorreu um erro não tratado: {Message}",
            exception.Message);

        // RFC 7807 ProblemDetails
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Erro Interno do Servidor",
            Detail = "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."
        };

        // TODO: Implementar para visualizar erro real ao estar desenvolvendo
        // if (environment.IsDevelopment()) problemDetails.Detail = exception.Message;

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}