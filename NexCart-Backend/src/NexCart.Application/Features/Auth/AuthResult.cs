using NexCart.Application.DTOs;
using NexCart.Contracts.Auth;

namespace NexCart.Application.Features.Auth;

public sealed record AuthResult(UserResponse User, AuthToken Token);
