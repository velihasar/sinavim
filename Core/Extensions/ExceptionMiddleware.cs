using System;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using Core.Utilities.Messages;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Core.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(httpContext, e);
            }
        }


        private async Task HandleExceptionAsync(HttpContext httpContext, Exception e)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var path = httpContext.Request.Path.Value ?? "";
            string message;
            if (e is ValidationException ve)
            {
                _logger.LogWarning(ve, "ValidationException Path={Path}", path);
                var parts = ve.Errors
                    .Select(err => err.ErrorMessage)
                    .Where(m => !string.IsNullOrWhiteSpace(m))
                    .Distinct()
                    .ToList();
                message = parts.Count > 0
                    ? string.Join(" ", parts)
                    : "Bilgiler doğrulanamadı. Lütfen kontrol edip tekrar deneyin.";
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else if (e.GetType() == typeof(ApplicationException))
            {
                _logger.LogWarning(e, "ApplicationException Path={Path}", path);
                message = e.Message;
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else if (e.GetType() == typeof(UnauthorizedAccessException))
            {
                _logger.LogWarning(e, "UnauthorizedAccessException Path={Path}", path);
                message = e.Message;
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            else if (e.GetType() == typeof(SecurityException))
            {
                _logger.LogWarning(e, "SecurityException Path={Path}", path);
                message = e.Message;
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            else if (e.GetType() == typeof(NotSupportedException))
            {
                _logger.LogWarning(e, "NotSupportedException Path={Path}", path);
                message = e.Message;
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                _logger.LogError(e, "Unhandled exception → generic 500 response. Path={Path}", path);
                message = ExceptionMessage.InternalServerError;
            }
            await httpContext.Response.WriteAsync(message);
        }
    }
}