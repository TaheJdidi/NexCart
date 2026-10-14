using System.Net.Http.Json;
using FluentAssertions;
using NexCart.Contracts.Auth;
using NexCart.Contracts.Stores;
using NexCart.IntegrationTests.Infrastructure;

namespace NexCart.IntegrationTests.Features.Stores;

[Collection(ApiCollection.Name)]
public class MyStoresTests(NexCartApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateBrowserClient();

    [Fact]
    public async Task A_new_account_owns_no_Stores()
    {
        await _client.SignUpAsync();

        var me = await _client.GetFromJsonAsync<UserResponse>("/api/auth/me");

        me!.Stores.Should().BeEmpty();
    }

    [Fact]
    public async Task Me_lists_every_Store_the_account_owns()
    {
        await _client.SignUpAsync();
        var shoes = await _client.CreateStoreAsync(name: "Ahmed Shoes");
        var bags = await _client.CreateStoreAsync(name: "Ahmed Bags");

        var me = await _client.GetFromJsonAsync<UserResponse>("/api/auth/me");

        me!.Stores.Should().BeEquivalentTo(
        [
            new OwnedStoreResponse(shoes.Id, "Ahmed Shoes", shoes.Handle),
            new OwnedStoreResponse(bags.Id, "Ahmed Bags", bags.Handle)
        ]);
    }

    [Fact]
    public async Task Me_does_not_list_another_accounts_Stores()
    {
        var other = factory.CreateBrowserClient();
        await other.SignUpAsync();
        await other.CreateStoreAsync();
        await _client.SignUpAsync();

        var me = await _client.GetFromJsonAsync<UserResponse>("/api/auth/me");

        me!.Stores.Should().BeEmpty();
    }
}
