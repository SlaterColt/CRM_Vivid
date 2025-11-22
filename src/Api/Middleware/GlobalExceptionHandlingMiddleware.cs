using System.Net;
using System.Text.Json;
using CRM_Vivid.Application.Exceptions;
using FluentValidation;

namespace CRM_Vivid.Api.Middleware
{
  public class GlobalExceptionHandlingMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
      _next = next;
      _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      try
      {
        await _next(context);
      }
      catch (Exception ex)
      {
        await HandleExceptionAsync(context, ex);
      }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
      context.Response.ContentType = "application/json";
      var response = context.Response;

      object errorResponse = new
      {
        message = exception.Message,
        details = exception.StackTrace?.ToString() // Only include stack trace in dev
      };

      // You can create a more structured error response if you prefer

      switch (exception)
      {
        case NotFoundException ex:
          response.StatusCode = (int)HttpStatusCode.NotFound;
          errorResponse = new { message = ex.Message, details = ex.StackTrace?.ToString() };
          break;

        case ValidationException ex:
          response.StatusCode = (int)HttpStatusCode.BadRequest;
          // Format FluentValidation errors
          var validationErrors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
          errorResponse = new { message = "Validation Failed", errors = validationErrors, details = ex.StackTrace?.ToString() };
          break;

        case ArgumentException ex: // Catch our old exceptions if any are left
          response.StatusCode = (int)HttpStatusCode.BadRequest;
          errorResponse = new { message = ex.Message, details = ex.StackTrace?.ToString() };
          break;

        default:
          response.StatusCode = (int)HttpStatusCode.InternalServerError;
          _logger.LogError(exception, "An unhandled exception has occurred.");
          errorResponse = new { message = "Internal Server Error", details = exception.StackTrace?.ToString() };
          break;
      }

      // In a real production app, you'd hide 'details' based on Environment
      var result = JsonSerializer.Serialize(errorResponse);
      await context.Response.WriteAsync(result);
    }
  }
}