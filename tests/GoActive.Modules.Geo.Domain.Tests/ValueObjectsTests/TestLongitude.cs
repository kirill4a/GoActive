using FluentAssertions;

using GoActive.Modules.Geo.Domain.ValueObjects;

namespace GoActive.Modules.Geo.Domain.Tests.ValueObjectsTests;
public class TestLongitude
{
    [Theory]
    [InlineData(180.0001d)]
    [InlineData(-180.0001d)]
    public void CreateLongitude_WrongValue(double longitude)
    {
        FluentActions.Invoking(() => _ = new Longitude(longitude))
                     .Should()
                     .ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CreateLongitude_Ok()
    {
        var value = 19.10002d;
        var longitude = new Longitude(value);
        longitude.Value.Should().Be(value);
    }
}