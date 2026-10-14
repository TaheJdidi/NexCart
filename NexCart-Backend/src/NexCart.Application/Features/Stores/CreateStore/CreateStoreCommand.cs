using MediatR;
using NexCart.Contracts.Stores;

namespace NexCart.Application.Features.Stores.CreateStore;

public sealed record CreateStoreCommand(string OwnerAccountId, string Name, string Handle, string? Currency) : IRequest<StoreResponse>;
