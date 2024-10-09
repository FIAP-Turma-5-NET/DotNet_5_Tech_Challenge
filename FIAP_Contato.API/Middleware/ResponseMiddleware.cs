using System.Text.Json;

namespace FIAP_Contato.API.Middleware;

public class ResponseHandleMiddleware
{
    private ILogger<ResponseHandleMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ResponseHandleMiddleware(RequestDelegate next, ILogger<ResponseHandleMiddleware> logger)
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
                    
    public async Task HandleExceptionAsync(HttpContext context, Exception exception) 
    {
        _logger.LogError(exception, exception.Message);
        
        context.Response.ContentType = "application/json";
        if (exception.GetType().Name.Equals("InvalidOperationException"))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = exception.Message }));
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "Algo inesperado aconteceu! Por favor, tente mais tarde." }));
        }
    }
}

public static class ResponseMiddleware 
{
    public static IApplicationBuilder UseResponseHandleMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ResponseHandleMiddleware>();
    }

}