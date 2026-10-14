namespace NexCart.Domain.Stores;

/// <summary>One Merchant's shop on the platform. See CONTEXT.md.</summary>
public sealed class Store
{
    public const string DefaultCurrency = "TND";

    private Store()
    {
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    // Permanent: nothing changes it after creation
    public string Handle { get; private set; } = string.Empty;

    // Fixed after creation, since prices and Orders are recorded in it
    public string Currency { get; private set; } = DefaultCurrency;

    public string OwnerAccountId { get; private set; } = string.Empty;
    public StoreStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public static Store Create(string name, StoreHandle handle, string currency, string ownerAccountId, DateTime createdAtUtc) =>
        new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Handle = handle.Value,
            Currency = currency,
            OwnerAccountId = ownerAccountId,
            Status = StoreStatus.Active,
            CreatedAtUtc = createdAtUtc
        };
}
