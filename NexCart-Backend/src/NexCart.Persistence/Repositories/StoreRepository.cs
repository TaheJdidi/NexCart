using Microsoft.EntityFrameworkCore;
using NexCart.Application.Interfaces;
using NexCart.Contracts.Stores;
using NexCart.Domain.Stores;
using NexCart.Persistence.Contexts;

namespace NexCart.Persistence.Repositories;

public class StoreRepository(AppDbContext dbContext) : IStoreRepository
{
    public Task<bool> IsHandleTakenAsync(string handle, CancellationToken cancellationToken) =>
        dbContext.Stores.AnyAsync(s => s.Handle == handle, cancellationToken);

    public async Task<IReadOnlyList<OwnedStoreResponse>> ListOwnedByAsync(string accountId, CancellationToken cancellationToken) =>
        await dbContext.Stores
            .AsNoTracking()
            .Where(s => s.OwnerAccountId == accountId)
            .OrderBy(s => s.CreatedAtUtc)
            .Select(s => new OwnedStoreResponse(s.Id, s.Name, s.Handle))
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Store store, CancellationToken cancellationToken)
    {
        dbContext.Stores.Add(store);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
