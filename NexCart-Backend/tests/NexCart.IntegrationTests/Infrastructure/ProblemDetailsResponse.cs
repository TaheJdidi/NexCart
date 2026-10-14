namespace NexCart.IntegrationTests.Infrastructure;

/// <summary>The error body the API returns; Errors is keyed by camelCased field name.</summary>
public sealed record ProblemDetailsResponse(string? Title, int? Status, IDictionary<string, string[]>? Errors);
