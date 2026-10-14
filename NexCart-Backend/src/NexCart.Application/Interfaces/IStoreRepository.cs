using NexCart.Contracts.Stores;
using NexCart.Domain.Stores;

namespace NexCart.Application.Interfaces;

public interface IStoreRepository
{
    Task<bool> IsHandleTakenAsync(string handle, CancellationToken cancellationToken);

    /// <summary>The Stores the account owns as a Merchant, oldest first.</summary>
    Task<IReadOnlyList<OwnedStoreResponse>> ListOwnedByAsync(string accountId, CancellationToken cancellationToken);

    Task AddAsync(Store store, CancellationToken cancellationToken);
}
