using NexCart.Contracts.Stores;

namespace NexCart.Contracts.Auth;

/// <summary>Stores lists the Stores the account owns as a Merchant.</summary>
public sealed record UserResponse(string Id, string Email, string FirstName, string LastName, IReadOnlyList<OwnedStoreResponse> Stores);
