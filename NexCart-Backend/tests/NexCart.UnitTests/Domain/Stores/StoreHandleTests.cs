using FluentAssertions;
using NexCart.Domain.Stores;

namespace NexCart.UnitTests.Domain.Stores;

public class StoreHandleTests
{
    [Theory]
    [InlineData("abc")]                                          // shortest allowed
    [InlineData("ahmed-shoes")]
    [InlineData("shop-24-7")]
    [InlineData("a234567890123456789012345678901234567890")]     // longest allowed (40)
    public void A_well_formed_handle_is_accepted(string input)
    {
        var result = StoreHandle.Parse(input);

        result.Problem.Should().BeNull();
        result.Handle!.Value.Should().Be(input);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]                                           // too short
    [InlineData("a2345678901234567890123456789012345678901")]    // too long (41)
    [InlineData("-ahmed")]                                       // starts with a hyphen
    [InlineData("ahmed-")]                                       // ends with a hyphen
    [InlineData("ahmed--shoes")]                                 // double hyphen
    [InlineData("ahmed_shoes")]                                  // underscore
    [InlineData("ahmed shoes")]                                  // space
    [InlineData("ahmed.shoes")]                                  // dot
    [InlineData("chaussures-é")]                                 // non-ASCII letter
    public void A_malformed_handle_is_invalid(string input)
    {
        var result = StoreHandle.Parse(input);

        result.Handle.Should().BeNull();
        result.Problem.Should().Be(StoreHandleProblem.Invalid);
    }

    [Theory]
    [InlineData("www")]
    [InlineData("api")]
    [InlineData("admin")]
    [InlineData("app")]
    [InlineData("platform")]
    [InlineData("mail")]
    [InlineData("static")]
    [InlineData("assets")]
    public void A_handle_the_platform_uses_itself_is_reserved(string input)
    {
        var result = StoreHandle.Parse(input);

        result.Handle.Should().BeNull();
        result.Problem.Should().Be(StoreHandleProblem.Reserved);
    }

    [Theory]
    [InlineData("Ahmed-Shoes")]
    [InlineData("  ahmed-shoes  ")]
    public void A_handle_is_lowercased_and_trimmed(string input)
    {
        var result = StoreHandle.Parse(input);

        result.Problem.Should().BeNull();
        result.Handle!.Value.Should().Be("ahmed-shoes");
    }

    [Fact]
    public void A_reserved_handle_in_capitals_is_still_reserved()
    {
        var result = StoreHandle.Parse("ADMIN");

        result.Problem.Should().Be(StoreHandleProblem.Reserved);
    }
}
