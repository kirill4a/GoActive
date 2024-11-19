using FluentAssertions;

using GoActive.Modules.Geo.Domain.ValueObjects;

namespace GoActive.Modules.Geo.Domain.Tests.ValueObjectsTests;

public class TestTitle
{
    public static readonly TheoryData<string> WrongValues = new()
        {
            null!,
            "",
            " "
        };

    [Theory]
    [MemberData(nameof(WrongValues))]
    public void Create_FromWrongValue_ShouldThrowException(string title)
    {
        // Act
        var function = () => Title.FromValue(title);

        // Assert
        function.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_FromValidValue_ShouldCreated()
    {
        // Arrange
        var value = "A valid title";

        // Act
        var title = Title.FromValue(value);

        // Assert
        title.Value.Should().Be(value);
    }
}
