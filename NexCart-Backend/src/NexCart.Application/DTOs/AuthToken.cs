namespace NexCart.Application.DTOs;

public sealed record AuthToken(string Value, DateTime ExpiresAtUtc);
