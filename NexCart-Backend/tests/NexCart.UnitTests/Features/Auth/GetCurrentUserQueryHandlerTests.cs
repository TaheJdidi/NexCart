using FluentAssertions;
using Moq;
using NexCart.Application.DTOs;
using NexCart.Application.Features.Auth.GetCurrentUser;
using NexCart.Application.Interfaces;

namespace NexCart.UnitTests.Features.Auth;

public class GetCurrentUserQueryHandlerTests
{
    private readonly Mock<IIdentityService> _identityService = new();

    [Fact]
    public async Task Returns_the_user_when_it_exists()
    {
        _identityService
            .Setup(s => s.FindByIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthUser("user-1", "taha@example.com", "Taha", "Jdidi"));

        var result = await new GetCurrentUserQueryHandler(_identityService.Object)
            .Handle(new GetCurrentUserQuery("user-1"), CancellationToken.None);

        result.Email.Should().Be("taha@example.com");
    }

    [Fact]
    public async Task Deleted_user_is_treated_as_signed_out()
    {
        _identityService
            .Setup(s => s.FindByIdAsync("gone", It.IsAny<CancellationToken>()))
            .ReturnsAsync((AuthUser?)null);

        var act = () => new GetCurrentUserQueryHandler(_identityService.Object)
            .Handle(new GetCurrentUserQuery("gone"), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
