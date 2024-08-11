using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace AndreyevInterview.Middleware;

public class RequestLoggingMiddleware: IMiddleware
{
    private readonly ILogger _logger;

    public RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _logger.LogInformation($"Handling request: {context.Request.Path}, start date: {DateTime.Now}");

        var watch = Stopwatch.StartNew();    
        await next(context);
        watch.Stop();

        var elapsedMs = watch.ElapsedMilliseconds;
        _logger.LogInformation($"Finished handling request: {context.Request.Path}, end date: {DateTime.Now}, elapsedMs: {elapsedMs}");
    }
}
