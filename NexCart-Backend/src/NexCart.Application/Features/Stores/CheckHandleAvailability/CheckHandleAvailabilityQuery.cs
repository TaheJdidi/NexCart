using MediatR;
using NexCart.Contracts.Stores;

namespace NexCart.Application.Features.Stores.CheckHandleAvailability;

public sealed record CheckHandleAvailabilityQuery(string Handle) : IRequest<HandleAvailabilityResponse>;
