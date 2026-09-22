using Microsoft.AspNetCore.Http;

namespace NexCart.Infrastructure.Authentication;

/// <summary>
/// The access token travels in an httpOnly cookie so browser scripts can never read it.
/// </summary>
public static class AuthCookie
{
    public const string Name = "nexcart_auth";

    public static CookieOptions CreateOptions(DateTimeOffset? expires = null) => new()
    {
        HttpOnly = true,
        // Browsers treat localhost as secure, so this also works over http during development
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Path = "/api",
        Expires = expires
    };
}
