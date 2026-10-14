namespace NexCart.Contracts.Stores;

public sealed record StoreResponse(Guid Id, string Name, string Handle, string Currency, string Status, DateTime CreatedAtUtc);
