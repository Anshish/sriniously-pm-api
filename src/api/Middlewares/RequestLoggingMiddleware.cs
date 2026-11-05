using System.Diagnostics;

namespace Api.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Check if the client sent an "X-Correlation-Id" header
        string correlationId = context.Request.Headers["X-Correlation-Id"].ToString();

        // 2. If they didn't send one, generate a fresh unique ID
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString("N"); // e.g. "4b9f2e71c84d..."
        }

        // 3. Store this ID in context.Items so our ExceptionMiddleware can read it if something crashes
        context.Items["CorrelationId"] = correlationId;

        // 4. Attach the ID to the response header so the client gets it back
        context.Response.Headers["X-Correlation-Id"] = correlationId;

        // 5. Start a stopwatch timer
        var timer = Stopwatch.StartNew();

        // 6. Log that the request has arrived
        logger.LogInformation("[{Id}] --> START: {Method} {Path}",
            correlationId, context.Request.Method, context.Request.Path);

        // 7. Hand the request over to the next step (Controllers, Services, Database)
        await next(context);

        // 8. The controller has finished! Stop the timer
        timer.Stop();

        // 9. Log that the request is complete, along with the HTTP status and time taken
        logger.LogInformation("[{Id}] <-- FINISHED: {Method} {Path} returned {StatusCode} in {Elapsed}ms",
            correlationId, context.Request.Method, context.Request.Path, context.Response.StatusCode, timer.ElapsedMilliseconds);
    }
}