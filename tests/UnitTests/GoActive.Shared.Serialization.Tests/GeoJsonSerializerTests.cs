using FluentAssertions;

using GoActive.Shared.Serialization.NetTopologySuite;
using GoActive.Shared.Serialization.Tests.TestData;

namespace GoActive.Shared.Serialization.Tests;

public class GeoJsonSerializerTests
{
    [Fact]
    public void Serialize_WhenCorrectFeature_ShouldReturnCorrectJson()
    {
        // Arrange
        var expectedJson =
"""{"type":"Feature","geometry":{"type":"Point","coordinates":[1,2,3]},"properties":{"firstName":"John","lastName":"Doe","activities":["NordicSki","Workout"]}}""";
        var serializer = new GeoJsonSerializer(writeIndented: false);

        // Act
        var json = serializer.SerializeFeature(new FakeSpotFeature());

        // Assert
        json.Should().Be(expectedJson);
    }
}
