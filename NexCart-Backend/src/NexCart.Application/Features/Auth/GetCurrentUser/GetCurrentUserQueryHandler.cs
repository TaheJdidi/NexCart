using MediatR;
using NexCart.Application.Interfaces;
using NexCart.Application.Mappings;
using NexCart.Contracts.Auth;

namespace NexCart.Application.Features.Auth.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler(IIdentityService identityService)
    : IRequestHandler<GetCurrentUserQuery, UserResponse>
{
    public async Task<UserResponse> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        // A valid token for a user that no longer exists is treated as signed out
        var user = await identityService.FindByIdAsync(request.UserId, cancellationToken)
            ?? throw new UnauthorizedAccessException("The user no longer exists.");

        return user.ToResponse();
    }
}
