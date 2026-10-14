using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NexCart.Persistence.Contexts;
using Testcontainers.MsSql;

namespace NexCart.IntegrationTests.Infrastructure;

/// <summary>
/// Boots the real API in memory against a real SQL Server running in a Docker container.
/// One instance is shared by every test in the <see cref="ApiCollection"/>, so tests must not
/// depend on each other's data: each one signs up its own NexCart Account with a unique email.
/// </summary>
public sealed class NexCartApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _database = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();

    public async Task InitializeAsync()
    {
        await _database.StartAsync();

        using var scope = Services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
    }

    /// <summary>
    /// A client that behaves like a browser: it keeps the httpOnly auth cookie between requests.
    /// The cookie is Secure, so the client must talk https for it to be sent back.
    /// </summary>
    public HttpClient CreateBrowserClient() => CreateClient(new WebApplicationFactoryClientOptions
    {
        BaseAddress = new Uri("https://localhost"),
        HandleCookies = true,
        AllowAutoRedirect = false
    });

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // UseSetting values reach Program.cs before it registers services, unlike ConfigureAppConfiguration
        builder.UseSetting("ConnectionStrings:DefaultConnection", _database.GetConnectionString());
        builder.UseSetting("Jwt:Secret", "integration-tests-secret-that-is-at-least-32-bytes");

        builder.ConfigureTestServices(RegisterFakes);
    }

    /// <summary>
    /// Replace services that reach outside the process (file storage, email, ...) with in-memory fakes here.
    /// </summary>
    private static void RegisterFakes(IServiceCollection services)
    {
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await DisposeAsync();
        await _database.DisposeAsync();
    }
}
