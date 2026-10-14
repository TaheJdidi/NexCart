using System.Net.Http.Json;
using FluentAssertions;
using NexCart.Contracts.Stores;
using NexCart.IntegrationTests.Infrastructure;

namespace NexCart.IntegrationTests.Features.Stores;

[Collection(ApiCollection.Name)]
public class HandleAvailabilityTests(NexCartApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateBrowserClient();

    [Fact]
    public async Task An_unused_handle_is_available()
    {
        var handle = StoreClientExtensions.UniqueHandle();

        var result = await _client.CheckHandleAsync(handle);

        result.Should().Be(new HandleAvailabilityResponse(handle, true, null));
    }

    [Fact]
    public async Task A_handle_used_by_another_Store_is_taken()
    {
        await _client.SignUpAsync();
        var store = await _client.CreateStoreAsync();

        var result = await factory.CreateBrowserClient().CheckHandleAsync(store.Handle);

        result.Should().Be(new HandleAvailabilityResponse(store.Handle, false, "taken"));
    }

    [Fact]
    public async Task A_malformed_handle_is_invalid()
    {
        var result = await _client.CheckHandleAsync("-ahmed--shoes");

        result.Should().Be(new HandleAvailabilityResponse("-ahmed--shoes", false, "invalid"));
    }

    [Fact]
    public async Task A_handle_the_platform_uses_itself_is_reserved()
    {
        var result = await _client.CheckHandleAsync("admin");

        result.Should().Be(new HandleAvailabilityResponse("admin", false, "reserved"));
    }

    [Fact]
    public async Task A_handle_is_taken_whatever_its_case()
    {
        await _client.SignUpAsync();
        var store = await _client.CreateStoreAsync();

        var result = await factory.CreateBrowserClient().CheckHandleAsync(store.Handle.ToUpperInvariant());

        result.Should().Be(new HandleAvailabilityResponse(store.Handle, false, "taken"));
    }
}
