using FluentAssertions;

using GoActive.Modules.Geo.Domain.ValueObjects;

namespace GoActive.Modules.Geo.Domain.Tests.ValueObjectsTests;
public class TestLatitude
{
    [Theory]
    [InlineData(90.0001d)]
    [InlineData([-90.0001d])]
    public void Create_FromWrongValue_ShouldThrowException(double latitude)
    {
        // Act
        var function = () => new Latitude(latitude);

        // Assert
        function.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Create_FromValidValue_ShouldCreated()
    {
        // Arrange
        var value = 42.11042d;

        // Act
        var latitude = new Latitude(value);

        // Assert
        latitude.Value.Should().Be(value);
    }
}