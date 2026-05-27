using Fcg.Identity.Domain.Items;
using Fcg.Identity.Domain.Results;
using FluentAssertions;

namespace Fcg.Identity.UnitTests.Domain.Items;

public sealed class ItemTests
{
    [Fact]
    public void Given_InvalidName_When_Create_Then_ShouldReturnValidationError()
    {
        var result = Item.Create(string.Empty, 10);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void Given_InvalidPrice_When_Create_Then_ShouldReturnValidationError()
    {
        var result = Item.Create("Notebook", 0);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void Given_ValidNameAndPrice_When_Create_Then_ShouldReturnSuccess()
    {
        var result = Item.Create("Notebook", 999.99m);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Notebook");
        result.Value.Price.Should().Be(999.99m);
    }
}
