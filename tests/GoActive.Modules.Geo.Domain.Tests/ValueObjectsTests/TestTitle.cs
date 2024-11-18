using FluentAssertions;

using GoActive.Modules.Geo.Domain.ValueObjects;

namespace GoActive.Modules.Geo.Domain.Tests.ValueObjectsTests;

public class TestTitle
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("a")]
    [InlineData(" test value ")]
    public void Create_FromWrongValue_ShouldThrowException(string title)
    {
        // Act
        var function = () => Title.FromValue(title);

        // Assert
        function.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    [Obsolete("Check all the wrong values in one method (via 'Should().Throw<ArgumentException>')")]
    public void Create_FromNullValue_ShouldThrowException()
    {
        // Arrange
        string? value = null;

        // Act
        var function = () => Title.FromValue(value!);

        // Assert
        function.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void Create_MaxlengthValue_ShouldThrowException()
    {
        // Arrange
        var value = new string('A', 101);

        // Act
        var function = () => Title.FromValue(value);

        // Assert
        function.Should().ThrowExactly<ArgumentException>();
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
