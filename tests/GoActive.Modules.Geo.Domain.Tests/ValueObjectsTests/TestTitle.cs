using FluentAssertions;

using GoActive.Modules.Geo.Domain.ValueObjects;

namespace GoActive.Modules.Geo.Domain.Tests.ValueObjectsTests;

public class TestTitle
{
    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("")]
    public void CreateTitle_WrongValue(string title)
    {
        FluentActions.Invoking(() => _ = Title.FromValue(title))
            .Should()
            .ThrowExactly<ArgumentException>()
            .WithMessage("Title cannot be null or whitespace.");
    }

    [Fact]
    public void CreateTitle_ValidValue()
    {
        var value = "A valid title";
        var title = Title.FromValue(value);
        title.Value.Should().Be(value);
    }
}
