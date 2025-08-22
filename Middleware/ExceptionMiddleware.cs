using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace JsonPlaceholderApi.Middleware
{
  public class ExceptionMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly Microsoft.Extensions.Logging.ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, Microsoft.Extensions.Logging.ILogger<ExceptionMiddleware> logger)
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
      catch (Exception ex)
      {
        _logger.LogError(ex, "Unhandled exception");
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        var result = JsonSerializer.Serialize(new { error = "Internal server error" });
        await httpContext.Response.WriteAsync(result);
      }
    }
  }
}
