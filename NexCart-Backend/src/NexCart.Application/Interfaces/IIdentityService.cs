using NexCart.Application.DTOs;

namespace NexCart.Application.Interfaces;

public interface IIdentityService
{
    /// <summary>Creates a user. Throws <see cref="FluentValidation.ValidationException"/> when the email is taken or the password is rejected.</summary>
    Task<AuthUser> CreateUserAsync(string firstName, string lastName, string email, string password, CancellationToken cancellationToken);

    /// <summary>Checks the credentials. Throws <see cref="UnauthorizedAccessException"/> when they are invalid or the account is locked out.</summary>
    Task<AuthUser> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken);

    Task<AuthUser?> FindByIdAsync(string userId, CancellationToken cancellationToken);
}
