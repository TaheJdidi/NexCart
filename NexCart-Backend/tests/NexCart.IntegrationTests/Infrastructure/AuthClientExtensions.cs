using System.Net;
using System.Net.Http.Json;
using NexCart.Contracts.Auth;

namespace NexCart.IntegrationTests.Infrastructure;

public static class AuthClientExtensions
{
    public const string DefaultPassword = "Passw0rd!";

    /// <summary>An email no other test uses, so tests never share data.</summary>
    public static string UniqueEmail() => $"account-{Guid.NewGuid():N}@example.com";

    /// <summary>A sign-up request with an email no other test uses.</summary>
    public static RegisterRequest NewSignUpRequest(string password = DefaultPassword) =>
        new("Taha", "Jdidi", UniqueEmail(), password);

    /// <summary>Signs up a new NexCart Account; the client is signed in as it afterwards.</summary>
    public static async Task<UserResponse> SignUpAsync(this HttpClient client, RegisterRequest? request = null)
    {
        var response = await client.PostAsJsonAsync("/api/auth/register", request ?? NewSignUpRequest());
        await EnsureStatusAsync(response, HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<UserResponse>())!;
    }

    /// <summary>Signs in as an existing NexCart Account; the client keeps the auth cookie.</summary>
    public static async Task<UserResponse> SignInAsync(this HttpClient client, string email, string password = DefaultPassword)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, password));
        await EnsureStatusAsync(response, HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<UserResponse>())!;
    }

    // Includes the response body in the failure, which EnsureSuccessStatusCode would hide
    private static async Task EnsureStatusAsync(HttpResponseMessage response, HttpStatusCode expected)
    {
        if (response.StatusCode != expected)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"{response.RequestMessage?.Method} {response.RequestMessage?.RequestUri} returned {(int)response.StatusCode}, expected {(int)expected}: {body}");
        }
    }
}
