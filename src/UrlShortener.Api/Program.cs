using Microsoft.EntityFrameworkCore;
using Serilog;
using UrlShortener.Api.Middleware;
using UrlShortener.Application;
using UrlShortener.Infrastructure;
using UrlShortener.Infrastructure.Persistence;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "UrlShortener")
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
        retainedFileCountLimit: 7)
    .CreateLogger();

try
{
    Log.Information("Starting URL Shortener API");

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog for logging
    builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();

// Register Application and Infrastructure layer services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "URL Shortener API",
        Version = "v1",
        Description = "A clean architecture URL shortening service"
    });
});

var app = builder.Build();

// Apply pending migrations on startup (for containerized environments)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
// Global exception handling (must be first)
app.UseMiddleware<ErrorHandlingMiddleware>();

// Enable Swagger in all environments (for demo/showcase purposes)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "URL Shortener API v1");
    options.RoutePrefix = "swagger"; // Swagger UI at /swagger
});

// Serilog request logging
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
    };
});

// For Docker, we may need to disable HTTPS redirection
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

    await app.RunAsync();
    
    Log.Information("URL Shortener API stopped cleanly");
}
catch (Exception ex)
{
    Log.Fatal(ex, "URL Shortener API terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

// Partial class for integration testing support
public partial class Program { }
