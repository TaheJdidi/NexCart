namespace NexCart.Contracts.Stores;

/// <summary>Reason is null when the handle is available, otherwise "taken", "reserved" or "invalid".</summary>
public sealed record HandleAvailabilityResponse(string Handle, bool Available, string? Reason);
