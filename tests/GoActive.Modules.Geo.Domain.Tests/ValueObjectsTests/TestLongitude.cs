using FluentAssertions;

using GoActive.Modules.Geo.Domain.ValueObjects;

namespace GoActive.Modules.Geo.Domain.Tests.ValueObjectsTests;
public class TestLongitude
{
    [Theory]
    [InlineData(180.0001d)]
    [InlineData(-180.0001d)]
    public void Create_FromWrongValue_ShouldThrowException(double longitude)
    {
        // Act
        var function = () => new Longitude(longitude);

        // Assert
        function.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Create_FromValidValue_ShouldCreated()
    {
        // Arrange
        var value = 19.10002d;

        // Act
        var longitude = new Longitude(value);

        // Assert
        longitude.Value.Should().Be(value);
    }
}