using MediatR;
using NexCart.Application.Interfaces;
using NexCart.Contracts.Stores;
using NexCart.Domain.Stores;

namespace NexCart.Application.Features.Stores.CheckHandleAvailability;

public sealed class CheckHandleAvailabilityQueryHandler(IStoreRepository stores)
    : IRequestHandler<CheckHandleAvailabilityQuery, HandleAvailabilityResponse>
{
    public async Task<HandleAvailabilityResponse> Handle(CheckHandleAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var (handle, problem) = StoreHandle.Parse(request.Handle);

        if (handle is null)
        {
            return new HandleAvailabilityResponse(request.Handle, false, problem!.Value.ToReason());
        }

        if (await stores.IsHandleTakenAsync(handle.Value, cancellationToken))
        {
            return new HandleAvailabilityResponse(handle.Value, false, StoreHandleReasons.Taken);
        }

        return new HandleAvailabilityResponse(handle.Value, true, null);
    }
}
