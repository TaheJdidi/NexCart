using MediatR;
using NexCart.Contracts.Auth;

namespace NexCart.Application.Features.Auth.GetCurrentUser;

public sealed record GetCurrentUserQuery(string UserId) : IRequest<UserResponse>;
