namespace UserManagementAPI.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log incoming request
            _logger.LogInformation(
                "[{Time}] REQUEST: {Method} {Path} | IP: {IP}",
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                context.Request.Method,
                context.Request.Path,
                context.Connection.RemoteIpAddress
            );

            var startTime = DateTime.UtcNow;

            await _next(context); // Call next middleware

            var elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;

            // Log response
            _logger.LogInformation(
                "[{Time}] RESPONSE: {StatusCode} | Duration: {Elapsed}ms",
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                context.Response.StatusCode,
                elapsed
            );
        }
    }

    // Extension method for clean registration in Program.cs
    public static class LoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoggingMiddleware>();
        }
    }
}
