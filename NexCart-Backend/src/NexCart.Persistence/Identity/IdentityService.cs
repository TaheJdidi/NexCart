using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using NexCart.Application.DTOs;
using NexCart.Application.Interfaces;

namespace NexCart.Persistence.Identity;

public class IdentityService(UserManager<ApplicationUser> userManager) : IIdentityService
{
    private const string InvalidCredentialsMessage = "Invalid email or password.";
    private const string LockedOutMessage = "Too many failed attempts. Please try again later.";

    public async Task<AuthUser> CreateUserAsync(string firstName, string lastName, string email, string password, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            CreatedAtUtc = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            // The user name is the email, so user-name errors duplicate the email ones
            var failures = result.Errors
                .Where(e => e.Code != nameof(IdentityErrorDescriber.DuplicateUserName))
                .Select(ToValidationFailure)
                .ToList();

            throw new ValidationException(failures);
        }

        return ToAuthUser(user);
    }

    public async Task<AuthUser> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email)
            ?? throw new UnauthorizedAccessException(InvalidCredentialsMessage);

        if (await userManager.IsLockedOutAsync(user))
        {
            throw new UnauthorizedAccessException(LockedOutMessage);
        }

        if (!await userManager.CheckPasswordAsync(user, password))
        {
            // Counts the failure and locks the account once the configured limit is reached
            await userManager.AccessFailedAsync(user);

            throw new UnauthorizedAccessException(
                await userManager.IsLockedOutAsync(user) ? LockedOutMessage : InvalidCredentialsMessage);
        }

        if (await userManager.GetAccessFailedCountAsync(user) > 0)
        {
            await userManager.ResetAccessFailedCountAsync(user);
        }

        return ToAuthUser(user);
    }

    public async Task<AuthUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user is null ? null : ToAuthUser(user);
    }

    private static AuthUser ToAuthUser(ApplicationUser user) =>
        new(user.Id, user.Email!, user.FirstName, user.LastName);

    private static ValidationFailure ToValidationFailure(IdentityError error)
    {
        var propertyName = error.Code switch
        {
            nameof(IdentityErrorDescriber.DuplicateEmail) or nameof(IdentityErrorDescriber.InvalidEmail) => "Email",
            _ when error.Code.StartsWith("Password", StringComparison.Ordinal) => "Password",
            _ => string.Empty
        };

        return new ValidationFailure(propertyName, error.Description);
    }
}
