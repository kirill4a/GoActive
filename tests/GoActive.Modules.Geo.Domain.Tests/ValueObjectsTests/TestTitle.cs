using FluentAssertions;

using GoActive.Modules.Geo.Domain.ValueObjects;

namespace GoActive.Modules.Geo.Domain.Tests.ValueObjectsTests;

public class TestTitle
{
    [Theory]
    [InlineData(" ")]
    [InlineData("")]
    public void CreateTitle_WrongValue(string title)
    {
        FluentActions.Invoking(() => _ = Title.FromValue(title))
            .Should()
            .ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void CreateTitle_ValidValue()
    {
        var value = "A valid title";
        var title = Title.FromValue(value);
        title.Value.Should().Be(value);
    }
}
