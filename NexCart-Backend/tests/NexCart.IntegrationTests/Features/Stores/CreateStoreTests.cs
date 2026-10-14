using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using NexCart.Contracts.Stores;
using NexCart.IntegrationTests.Infrastructure;

namespace NexCart.IntegrationTests.Features.Stores;

[Collection(ApiCollection.Name)]
public class CreateStoreTests(NexCartApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateBrowserClient();

    [Fact]
    public async Task A_signed_in_account_creates_a_Store()
    {
        await _client.SignUpAsync();
        var handle = StoreClientExtensions.UniqueHandle();

        var response = await _client.PostAsJsonAsync("/api/stores", new CreateStoreRequest("Ahmed Shoes", handle));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var store = await response.Content.ReadFromJsonAsync<StoreResponse>();
        store!.Name.Should().Be("Ahmed Shoes");
        store.Handle.Should().Be(handle);
        store.Currency.Should().Be("TND");
        store.Status.Should().Be("Active");
    }

    [Fact]
    public async Task A_Store_cannot_take_a_handle_that_is_already_used()
    {
        var other = factory.CreateBrowserClient();
        await other.SignUpAsync();
        var existing = await other.CreateStoreAsync();
        await _client.SignUpAsync();

        var response = await _client.PostAsJsonAsync("/api/stores", new CreateStoreRequest("Copy", existing.Handle));

        await response.ShouldBeRejectedOnHandleAsync();
    }

    [Fact]
    public async Task A_Store_cannot_have_a_malformed_handle()
    {
        await _client.SignUpAsync();

        var response = await _client.PostAsJsonAsync("/api/stores", new CreateStoreRequest("Ahmed Shoes", "ahmed_shoes"));

        await response.ShouldBeRejectedOnHandleAsync();
    }

    [Fact]
    public async Task A_Store_cannot_take_a_reserved_handle()
    {
        await _client.SignUpAsync();

        var response = await _client.PostAsJsonAsync("/api/stores", new CreateStoreRequest("Admin", "admin"));

        await response.ShouldBeRejectedOnHandleAsync();
    }

    [Fact]
    public async Task A_handle_typed_in_capitals_is_kept_in_lowercase()
    {
        await _client.SignUpAsync();
        var handle = StoreClientExtensions.UniqueHandle();

        var store = await _client.CreateStoreAsync(handle.ToUpperInvariant());

        store.Handle.Should().Be(handle);
    }

    [Fact]
    public async Task Creating_a_Store_requires_a_signed_in_account()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/stores", new CreateStoreRequest("Ahmed Shoes", StoreClientExtensions.UniqueHandle()));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task A_Store_can_be_created_in_another_currency()
    {
        await _client.SignUpAsync();

        var response = await _client.PostAsJsonAsync(
            "/api/stores", new CreateStoreRequest("Ahmed Shoes", StoreClientExtensions.UniqueHandle(), "EUR"));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        (await response.Content.ReadFromJsonAsync<StoreResponse>())!.Currency.Should().Be("EUR");
    }

    [Theory]
    [InlineData("XYZ")]     // not an ISO 4217 code
    [InlineData("EURO")]
    [InlineData("eu")]
    public async Task A_Store_cannot_use_an_unknown_currency(string currency)
    {
        await _client.SignUpAsync();

        var response = await _client.PostAsJsonAsync(
            "/api/stores", new CreateStoreRequest("Ahmed Shoes", StoreClientExtensions.UniqueHandle(), currency));

        await response.ShouldBeRejectedOnAsync("currency");
    }

    [Fact]
    public async Task A_Store_needs_a_name()
    {
        await _client.SignUpAsync();

        var response = await _client.PostAsJsonAsync(
            "/api/stores", new CreateStoreRequest("  ", StoreClientExtensions.UniqueHandle()));

        await response.ShouldBeRejectedOnAsync("name");
    }
}
