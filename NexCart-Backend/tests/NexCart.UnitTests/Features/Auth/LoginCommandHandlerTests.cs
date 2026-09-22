using FluentAssertions;
using Moq;
using NexCart.Application.DTOs;
using NexCart.Application.Features.Auth.Login;
using NexCart.Application.Interfaces;

namespace NexCart.UnitTests.Features.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityService = new();
    private readonly Mock<IJwtTokenGenerator> _tokenGenerator = new();
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(_identityService.Object, _tokenGenerator.Object);
    }

    [Fact]
    public async Task Valid_credentials_return_the_user_and_a_token()
    {
        var user = new AuthUser("user-1", "taha@example.com", "Taha", "Jdidi");
        var token = new AuthToken("jwt", DateTime.UtcNow.AddHours(1));
        _identityService
            .Setup(s => s.ValidateCredentialsAsync("taha@example.com", "Passw0rd!", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _tokenGenerator.Setup(g => g.Generate(user)).Returns(token);

        // Surrounding whitespace in the email is ignored
        var result = await _handler.Handle(new LoginCommand("  taha@example.com ", "Passw0rd!"), CancellationToken.None);

        result.User.Id.Should().Be("user-1");
        result.User.FirstName.Should().Be("Taha");
        result.Token.Should().Be(token);
    }

    [Fact]
    public async Task Invalid_credentials_do_not_issue_a_token()
    {
        _identityService
            .Setup(s => s.ValidateCredentialsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid email or password."));

        var act = () => _handler.Handle(new LoginCommand("taha@example.com", "wrong"), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        _tokenGenerator.Verify(g => g.Generate(It.IsAny<AuthUser>()), Times.Never);
    }
}
