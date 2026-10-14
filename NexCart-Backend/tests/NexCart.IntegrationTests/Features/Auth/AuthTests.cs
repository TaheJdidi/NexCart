using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using NexCart.Contracts.Auth;
using NexCart.IntegrationTests.Infrastructure;

namespace NexCart.IntegrationTests.Features.Auth;

[Collection(ApiCollection.Name)]
public class AuthTests(NexCartApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateBrowserClient();

    [Fact]
    public async Task Register_creates_the_account_and_signs_it_in()
    {
        var request = AuthClientExtensions.NewSignUpRequest();

        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var me = await _client.GetFromJsonAsync<UserResponse>("/api/auth/me");
        me!.Email.Should().Be(request.Email);
        me.FirstName.Should().Be(request.FirstName);
        me.LastName.Should().Be(request.LastName);
    }

    [Fact]
    public async Task Register_rejects_an_email_that_is_already_used()
    {
        var request = AuthClientExtensions.NewSignUpRequest();
        await factory.CreateBrowserClient().SignUpAsync(request);

        var response = await _client.PostAsJsonAsync("/api/auth/register", request with { FirstName = "Other" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await _client.GetAsync("/api/auth/me")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_with_valid_credentials_signs_the_account_in()
    {
        var request = AuthClientExtensions.NewSignUpRequest();
        await factory.CreateBrowserClient().SignUpAsync(request);

        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest(request.Email, request.Password));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var me = await _client.GetFromJsonAsync<UserResponse>("/api/auth/me");
        me!.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task Login_with_a_wrong_password_is_unauthorized()
    {
        var request = AuthClientExtensions.NewSignUpRequest();
        await factory.CreateBrowserClient().SignUpAsync(request);

        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest(request.Email, "Wrong-passw0rd!"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await _client.GetAsync("/api/auth/me")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_with_an_unknown_email_is_unauthorized()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest(AuthClientExtensions.UniqueEmail(), AuthClientExtensions.DefaultPassword));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Me_without_the_auth_cookie_is_unauthorized()
    {
        var response = await _client.GetAsync("/api/auth/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Logout_clears_the_auth_cookie()
    {
        await _client.SignUpAsync();

        var response = await _client.PostAsync("/api/auth/logout", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await _client.GetAsync("/api/auth/me")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Signing_in_again_after_logout_restores_access()
    {
        var account = await _client.SignUpAsync();
        await _client.PostAsync("/api/auth/logout", null);

        await _client.SignInAsync(account.Email);

        var me = await _client.GetFromJsonAsync<UserResponse>("/api/auth/me");
        me!.Id.Should().Be(account.Id);
    }
}
