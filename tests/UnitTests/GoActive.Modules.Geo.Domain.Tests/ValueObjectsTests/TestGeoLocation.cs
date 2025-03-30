using FluentAssertions;

using GoActive.Modules.Geo.Domain.ValueObjects;

namespace GoActive.Modules.Geo.Domain.Tests.ValueObjectsTests;

public class TestGeoLocation
{
    [Theory]
    [InlineData(-90.0001d, 25.0d)]
    [InlineData([60.0001d, 180.00001d])]
    public void Create_FromWrongValues_ShouldThrowException(double latitude, double longitude)
    {
        // Act
        var function = () => GeoLocation.FromLatLon(latitude, longitude);

        // Assert
        function.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Create_FromLatLon_ShouldCreated()
    {
        // Arrange
        var latitude = 66.91714474903289;
        var longitude = 17.67141915518521;

        // Act
        var coordinate = GeoLocation.FromLatLon(latitude, longitude);

        // Assert
        coordinate.Should().NotBeNull();
        coordinate.Should().NotBe(default(GeoLocation));
        coordinate.Latitude.Value.Should().Be(latitude);
        coordinate.Longitude.Value.Should().Be(longitude);
    }
}
