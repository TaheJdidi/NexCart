using Microsoft.AspNetCore.StaticFiles;
using NexCart.Application;
using NexCart.Infrastructure;
using NexCart.Persistence;
using Serilog;

const string logTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}";
var logFilePath = GetLogFilePath();

// Bootstrap logger: captures errors that happen during startup, before the host's logger exists
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: logTemplate)
    .WriteTo.File(path: logFilePath, rollingInterval: RollingInterval.Day, outputTemplate: logTemplate)
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog
    builder.Host.UseSerilog((context, configuration) =>
        configuration
            .MinimumLevel.Information()
            .WriteTo.Console(outputTemplate: logTemplate)
            .WriteTo.File(path: logFilePath, rollingInterval: RollingInterval.Day, outputTemplate: logTemplate)
    );

    builder.Services
        .AddApplication()
        .AddPersistence(builder.Configuration)
        .AddInfrastructure(builder.Configuration);

    // Let FluentValidation report missing fields instead of MVC's implicit [Required],
    // so every validation error has the same response shape
    builder.Services.AddControllers(options =>
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

    // Used by the App Service health check to detect and restart an unhealthy instance
    builder.Services.AddHealthChecks();

    var app = builder.Build();

    // Add global exception handling middleware
    app.UseGlobalExceptionHandler();

    // Serve the React app, which is published into wwwroot/ (see NexCart.API.csproj).
    // In development wwwroot/ is empty and the Vite dev server serves the frontend instead.
    var frontendFiles = CreateFrontendFileOptions();
    app.UseDefaultFiles();
    app.UseStaticFiles(frontendFiles);

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapHealthChecks("/health");

    // Unknown API routes are real 404s; every other path (/login, /register, ...) is a
    // React Router page, so it gets index.html and the client-side router takes over
    app.MapFallback("/api/{**path}", () => Results.NotFound());
    app.MapFallbackToFile("index.html", frontendFiles);

    app.Run();
}
// EF Core tools stop the host on purpose after reading the configuration; that is not a failure
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application failed to start");
    Environment.ExitCode = 1;
}
finally
{
    Log.CloseAndFlush();
}

// On Azure App Service, write to %HOME%/LogFiles/Application: it survives restarts and
// deployments, and the portal's Log stream shows it. Locally, keep the logs/ folder.
static string GetLogFilePath()
{
    var home = Environment.GetEnvironmentVariable("HOME");
    var isAppService = Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME") is not null;

    return isAppService && !string.IsNullOrEmpty(home)
        ? Path.Combine(home, "LogFiles", "Application", "nxcart-.txt")
        : Path.Combine("logs", "nxcart-.txt");
}

// Vite puts a content hash in every file name under /assets, so those can be cached forever.
// index.html must always be revalidated, or browsers keep loading the previous deployment.
static StaticFileOptions CreateFrontendFileOptions() => new()
{
    OnPrepareResponse = context =>
    {
        var isHashedAsset = context.Context.Request.Path.StartsWithSegments("/assets");

        context.Context.Response.Headers.CacheControl = isHashedAsset
            ? "public, max-age=31536000, immutable"
            : "no-cache";
    }
};
