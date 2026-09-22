using NexCart.Application.DTOs;

namespace NexCart.Application.Interfaces;

public interface IJwtTokenGenerator
{
    AuthToken Generate(AuthUser user);
}
