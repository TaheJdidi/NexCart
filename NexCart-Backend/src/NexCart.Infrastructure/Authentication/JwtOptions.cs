namespace NexCart.Infrastructure.Authentication;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    /// <summary>HMAC signing key. Keep it out of appsettings: use user secrets or an environment variable.</summary>
    public string Secret { get; init; } = string.Empty;

    public int ExpiryMinutes { get; init; } = 60;
}
