namespace UserManagementAPI.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthMiddleware> _logger;

        // Hardcoded API key for demo purposes
        // In production: store in environment variables / secrets manager
        private const string API_KEY_HEADER = "X-API-Key";
        private const string VALID_API_KEY = "my-secret-api-key-123";

        public AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip auth for Swagger UI and health check
            var path = context.Request.Path.Value ?? "";
            if (path.StartsWith("/swagger") || path == "/health")
            {
                await _next(context);
                return;
            }

            // Check for API key in request headers
            if (!context.Request.Headers.TryGetValue(API_KEY_HEADER, out var extractedKey))
            {
                _logger.LogWarning("Request rejected: Missing API key | Path: {Path}", path);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Unauthorized",
                    message = "API key is missing. Add 'X-API-Key' header."
                });
                return;
            }

            if (extractedKey != VALID_API_KEY)
            {
                _logger.LogWarning("Request rejected: Invalid API key | Path: {Path}", path);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Unauthorized",
                    message = "Invalid API key."
                });
                return;
            }

            // Valid key — proceed
            await _next(context);
        }
    }

    public static class AuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiKeyAuth(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AuthMiddleware>();
        }
    }
}
