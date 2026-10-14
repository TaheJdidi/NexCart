using System.Net;
using System.Net.Http.Json;
using NexCart.Contracts.Stores;

namespace NexCart.IntegrationTests.Infrastructure;

public static class StoreClientExtensions
{
    /// <summary>A Store Handle no other test uses, so tests never collide on the platform-wide uniqueness rule.</summary>
    public static string UniqueHandle() => $"store-{Guid.NewGuid():N}"[..20];

    /// <summary>Creates a Store owned by the signed-in account, with a handle no other test uses unless one is given.</summary>
    public static async Task<StoreResponse> CreateStoreAsync(this HttpClient client, string? handle = null, string name = "Ahmed Shoes")
    {
        var response = await client.PostAsJsonAsync("/api/stores", new CreateStoreRequest(name, handle ?? UniqueHandle()));
        await response.EnsureStatusAsync(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<StoreResponse>())!;
    }

    public static async Task<HandleAvailabilityResponse> CheckHandleAsync(this HttpClient client, string handle) =>
        (await client.GetFromJsonAsync<HandleAvailabilityResponse>(
            $"/api/stores/handle-availability?handle={Uri.EscapeDataString(handle)}"))!;
}
