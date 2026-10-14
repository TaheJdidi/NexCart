using MediatR;
using NexCart.Application.Interfaces;
using NexCart.Application.Mappings;
using NexCart.Contracts.Stores;
using NexCart.Domain.Stores;

namespace NexCart.Application.Features.Stores.CreateStore;

public sealed class CreateStoreCommandHandler(IStoreRepository stores) : IRequestHandler<CreateStoreCommand, StoreResponse>
{
    public async Task<StoreResponse> Handle(CreateStoreCommand request, CancellationToken cancellationToken)
    {
        // The validator has already rejected a handle with a problem
        var handle = StoreHandle.Parse(request.Handle).Handle!;

        var store = Store.Create(
            request.Name.Trim(),
            handle,
            request.Currency ?? Store.DefaultCurrency,
            request.OwnerAccountId,
            DateTime.UtcNow);

        await stores.AddAsync(store, cancellationToken);

        return store.ToResponse();
    }
}
