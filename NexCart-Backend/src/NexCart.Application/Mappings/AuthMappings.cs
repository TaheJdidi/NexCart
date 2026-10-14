using NexCart.Application.DTOs;
using NexCart.Contracts.Auth;
using NexCart.Contracts.Stores;

namespace NexCart.Application.Mappings;

public static class AuthMappings
{
    /// <summary>Stores defaults to none, which is right for an account that has just signed up.</summary>
    public static UserResponse ToResponse(this AuthUser user, IReadOnlyList<OwnedStoreResponse>? stores = null) =>
        new(user.Id, user.Email, user.FirstName, user.LastName, stores ?? []);
}
