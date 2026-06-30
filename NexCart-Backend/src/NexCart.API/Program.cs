using NexCart.Application;
using NexCart.Infrastructure;
using NexCart.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddPersistence(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Add global exception handling middleware
app.UseGlobalExceptionHandler();

app.MapGet("/", () => "Hello World!");

app.Run();
