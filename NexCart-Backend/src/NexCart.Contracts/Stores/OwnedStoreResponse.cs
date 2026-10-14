namespace NexCart.Contracts.Stores;

/// <summary>A Store as listed on the Merchant's account, enough to link to its admin area and site.</summary>
public sealed record OwnedStoreResponse(Guid Id, string Name, string Handle);
