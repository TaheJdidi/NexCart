namespace NexCart.Contracts.Stores;

/// <summary>Currency is an ISO 4217 code; when omitted the Store uses TND.</summary>
public sealed record CreateStoreRequest(string Name, string Handle, string? Currency = null);
