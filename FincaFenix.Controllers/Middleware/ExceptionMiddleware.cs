using System.Net;
using FincaFenix.Entities.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FincaFenixControllers.Middleware
{
    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (NotFoundException ex)
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.NotFound, "No se encontró el recurso solicitado.");
            }
            catch (ValidationException ex)
            {
                await HandleValidationExceptionAsync(context, ex);
            }
            catch (UnauthorizedAccessException)
            {
                await HandleExceptionAsync(context, null, HttpStatusCode.Forbidden, "No tiene permisos para acceder a este recurso.");
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError, "Ocurrió un error interno en el servidor.");
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex, HttpStatusCode statusCode, string detail)
        {
            if (ex is not null)
                logger.LogError(ex, "Error {StatusCode}: {Message}", (int)statusCode, ex.Message);

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)statusCode;

            var problem = new ProblemDetails
            {
                Title = GetTitleForStatusCode(statusCode),
                Status = (int)statusCode,
                Detail = detail,
                Instance = context.Request.Path
            };

            return context.Response.WriteAsJsonAsync(problem);
        }

        private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException ex)
        {
            logger.LogWarning("Validation error: {Errors}", string.Join("; ", ex.Errors.Select(e => e.ErrorMessage)));

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;

            var problem = new ProblemDetails
            {
                Title = "Error de validación",
                Status = (int)HttpStatusCode.UnprocessableEntity,
                Detail = "Uno o más campos no son válidos.",
                Instance = context.Request.Path,
                Extensions = { ["errors"] = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) }
            };

            await context.Response.WriteAsJsonAsync(problem);
        }

        private static string GetTitleForStatusCode(HttpStatusCode statusCode) => statusCode switch
        {
            HttpStatusCode.NotFound => "Recurso no encontrado",
            HttpStatusCode.Forbidden => "Acceso denegado",
            HttpStatusCode.InternalServerError => "Error interno del servidor",
            _ => "Error"
        };
    }
}
