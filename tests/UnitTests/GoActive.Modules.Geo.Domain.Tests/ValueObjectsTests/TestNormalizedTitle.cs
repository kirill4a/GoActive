using FluentAssertions;

using GoActive.Modules.Geo.Domain.ValueObjects;

namespace GoActive.Modules.Geo.Domain.Tests.ValueObjectsTests;

public class TestNormalizedTitle
{
    public static readonly TheoryData<string> WrongValues = new()
    {
        null!,
        string.Empty,
        "",
        " ",
    };

    [Theory]
    [MemberData(nameof(WrongValues))]
    public void Create_FromWrongValue_ShouldThrowException(string input)
    {
        // Act
        var action = () => NormalizedTitle.FromValue(input);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("title", "title")]
    [InlineData("tiTle", "title")]
    [InlineData("  A valid title  ", "avalidtitle")]
    [InlineData("A valiD    tiTle", "avalidtitle")]
    public void Create_FromValidValue_ShouldCreated(string input, string expected)
    {
        // Act
        var normalizedTitle = NormalizedTitle.FromValue(input);

        // Assert
        normalizedTitle.Value.Should().Be(expected);
    }
}
