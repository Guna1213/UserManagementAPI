using UserManagementAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ──────────────────────────────────────────────
// Register Services
// ──────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "User Management API",
        Version = "v1",
        Description = "A simple CRUD API for managing users with logging and auth middleware."
    });

    // Add API key input to Swagger UI
    options.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "X-API-Key",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Description = "Enter your API key. Use: my-secret-api-key-123"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// ──────────────────────────────────────────────
// Middleware Pipeline (order matters!)
// ──────────────────────────────────────────────

// 1. Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API v1");
        c.RoutePrefix = "swagger";
    });
}

// 2. Custom Logging Middleware — logs every request/response
app.UseRequestLogging();

// 3. Custom Auth Middleware — validates API key header
app.UseApiKeyAuth();

// 4. HTTPS Redirection
app.UseHttpsRedirection();

// 5. Route to controllers
app.MapControllers();

// Health check endpoint (no auth required)
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

app.Run();
