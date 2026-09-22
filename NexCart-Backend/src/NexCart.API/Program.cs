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

    // Let FluentValidation report missing fields instead of MVC's implicit [Required],
    // so every validation error has the same response shape
    builder.Services.AddControllers(options =>
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

    var app = builder.Build();

    // Add global exception handling middleware
    app.UseGlobalExceptionHandler();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

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
