using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using NexCart.IntegrationTests.Infrastructure;

namespace NexCart.IntegrationTests.Features.Stores;

public static class StoreAssertions
{
    public static Task ShouldBeRejectedOnHandleAsync(this HttpResponseMessage response) =>
        response.ShouldBeRejectedOnAsync("handle");

    public static async Task ShouldBeRejectedOnAsync(this HttpResponseMessage response, string field)
    {
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>();
        problem!.Errors.Should().ContainKey(field);
    }
}
