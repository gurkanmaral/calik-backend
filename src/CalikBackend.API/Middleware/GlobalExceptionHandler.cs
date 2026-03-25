using CalikBackend.Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CalikBackend.API.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var (statusCode, title, detail) = exception switch
        {
            NotFoundException ex      => (StatusCodes.Status404NotFound,    "Not Found",            ex.Message),
            BadRequestException ex    => (StatusCodes.Status400BadRequest,  "Bad Request",          ex.Message),
            ConflictException ex      => (StatusCodes.Status409Conflict,    "Conflict",             ex.Message),
            ValidationException ex    => (StatusCodes.Status422UnprocessableEntity, "Validation Error",
                string.Join("; ", ex.Errors.Select(e => e.ErrorMessage))),
            _                         => (StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred.")
        };

        httpContext.Response.StatusCode = statusCode;

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title  = title,
            Detail = detail
        };

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
