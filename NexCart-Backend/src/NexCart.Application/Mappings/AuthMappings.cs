using NexCart.Application.DTOs;
using NexCart.Contracts.Auth;

namespace NexCart.Application.Mappings;

public static class AuthMappings
{
    public static UserResponse ToResponse(this AuthUser user) =>
        new(user.Id, user.Email, user.FirstName, user.LastName);
}
