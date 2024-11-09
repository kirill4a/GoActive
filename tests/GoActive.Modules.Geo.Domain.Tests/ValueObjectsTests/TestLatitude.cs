using FluentAssertions;

using GoActive.Modules.Geo.Domain.ValueObjects;

namespace GoActive.Modules.Geo.Domain.Tests.ValueObjectsTests;
public class TestLatitude
{
    [Theory]
    [InlineData(90.0001d)]
    [InlineData([-90.0001d])]
    public void CreateLatitude_WrongValue(double latitude)
    {
        FluentActions.Invoking(() => _ = new Latitude(latitude))
                     .Should()
                     .ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CreateLatitude_Ok()
    {
        var value = 42.11042d;
        var latitude = new Latitude(value);
        latitude.Value.Should().Be(value);
    }
}