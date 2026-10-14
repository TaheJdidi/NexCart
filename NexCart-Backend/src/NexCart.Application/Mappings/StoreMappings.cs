using NexCart.Contracts.Stores;
using NexCart.Domain.Stores;

namespace NexCart.Application.Mappings;

public static class StoreMappings
{
    public static StoreResponse ToResponse(this Store store) =>
        new(store.Id, store.Name, store.Handle, store.Currency, store.Status.ToString(), store.CreatedAtUtc);
}
