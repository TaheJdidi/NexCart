using NexCart.Application;
using NexCart.Infrastructure;
using NexCart.Persistence;
using Serilog;

const string logTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}";

// Bootstrap logger: captures errors that happen during startup, before the host's logger exists
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: logTemplate)
    .WriteTo.File(path: "logs/nxcart-.txt", rollingInterval: RollingInterval.Day, outputTemplate: logTemplate)
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog
    builder.Host.UseSerilog((context, configuration) =>
        configuration
            .MinimumLevel.Information()
            .WriteTo.Console(outputTemplate: logTemplate)
            .WriteTo.File(path: "logs/nxcart-.txt", rollingInterval: RollingInterval.Day, outputTemplate: logTemplate)
    );

    builder.Services
        .AddApplication()
        .AddPersistence(builder.Configuration)
        .AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    // Add global exception handling middleware
    app.UseGlobalExceptionHandler();

    app.MapGet("/", () => "Hello World!");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}
