using FluentAssertions;
using NexCart.Application.Features.Auth.Register;

namespace NexCart.UnitTests.Features.Auth;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public void Valid_command_passes()
    {
        var result = _validator.Validate(new RegisterCommand("Taha", "Jdidi", "taha@example.com", "Passw0rd!"));

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("Sh0rt!")]       // too short
    [InlineData("passw0rd!")]    // no uppercase
    [InlineData("PASSW0RD!")]    // no lowercase
    [InlineData("Password!")]    // no digit
    [InlineData("Passw0rd1")]    // no special character
    public void Weak_password_fails(string password)
    {
        var result = _validator.Validate(new RegisterCommand("Taha", "Jdidi", "taha@example.com", password));

        result.Errors.Should().ContainSingle().Which.PropertyName.Should().Be(nameof(RegisterCommand.Password));
    }

    [Fact]
    public void Missing_names_and_invalid_email_fail()
    {
        var result = _validator.Validate(new RegisterCommand("", "", "not-an-email", "Passw0rd!"));

        result.Errors.Select(e => e.PropertyName).Should().BeEquivalentTo(
            nameof(RegisterCommand.FirstName), nameof(RegisterCommand.LastName), nameof(RegisterCommand.Email));
    }
}
