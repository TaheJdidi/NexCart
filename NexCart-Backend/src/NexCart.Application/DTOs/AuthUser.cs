namespace NexCart.Application.DTOs;

/// <summary>
/// A user as seen by the application layer, independent of how identity is stored.
/// </summary>
public sealed record AuthUser(string Id, string Email, string FirstName, string LastName);
