using System.Text.Json;

using FluentAssertions;

using GoActive.Shared.Serialization.NetTopologySuite;

using NetTopologySuite.Features;

namespace GoActive.Shared.Serialization.Tests;

public class GeoJsonDeserializerTests
{
    public static TheoryData<string> MalformedData => new()
    {
        "QWERTY",
        """
        {"field" "value"}
        """,
    };

    public static TheoryData<string> CorrectData => new()
    {
        """
        {"geometry": {"type": "Point", "coordinates": [15.7038223, 46.2244948]}}
        """,
        """
        {"type": "Feature", "properties": {"hash": "34ebd9543f0c0ebf", "number": "23", "street": "Steklarska ulica", "unit": "", "city": "Rogatec", "district": "Rogatec", "region": "Savinjska", "postcode": "3252", "id": "100400000152122093"}, "geometry": {"type": "Point", "coordinates": [15.7038223, 46.2244948]}}
        """,
        """
        {"type": "Feature", "properties": {"hash": "81d9957811fcf707", "number": "3", "street": "Cesta svobode", "unit": "", "city": "Bled", "district": "Bled", "region": "Gorenjska", "postcode": "4260", "id": "100400000144050204"}, "geometry": {"type": "Point", "coordinates": [14.1075464, 46.3709212]}}
        """,
    };

    [Fact]
    public void Deserialize_WhenTypeMismatch_ShouldThrowArgumentException()
    {
        // Arrange
        var serializer = new GeoJsonSerializer();

        // Act
        var act = () => serializer.Deserialize<object>("""{"field": "value"}""");

        // Assert
        act.Should().ThrowExactly<ArgumentException>()
           .WithMessage("Type 'System.Object' is not supported by GeoJsonSerializer.");
    }

    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData("    ")]
    public void Deserialize_WhenEmptyJson_ShouldThrowArgumentException(string? json)
    {
        // Arrange
        var serializer = new GeoJsonSerializer();

        // Act
        var act = () => serializer.Deserialize<IFeature>(json!);

        // Assert
        act.Should().ThrowExactly<ArgumentException>()
           .WithMessage("Value cannot be null, empty string or whitespace. (Parameter 'json')");
    }

    [Theory]
    [MemberData(nameof(MalformedData))]
    public void Deserialize_WhenMalformedJson_ShouldThrowJsonException(string json)
    {
        // Arrange
        var serializer = new GeoJsonSerializer();

        // Act
        var act = () => serializer.Deserialize<IFeature>(json);

        // Assert
        act.Should().ThrowExactly<JsonException>();
    }

    [Fact]
    public void Deserialize_WhenIncorrectFeature_ShouldThrowJsonException()
    {
        // Arrange
        var json =
        """
        {"type": "SomeType", "properties": {"id": "100400000152122093"}, "geometry": {"type": "Point", "coordinates": [15.7038223, 46.2244948]}}
        """;
        var serializer = new GeoJsonSerializer();

        // Act
        var act = () => serializer.Deserialize<IFeature>(json);

        // Assert
        act.Should().ThrowExactly<JsonException>()
           .WithMessage("Expected value 'Feature' not found.");
    }

    [Fact]
    public void Deserialize_WhenNotGeoJson_ShouldReturnEmptyData()
    {
        // Arrange
        var json =
        """
        {"field": "value"}
        """;
        var serializer = new GeoJsonSerializer();

        // Act
        var data = serializer.Deserialize<IFeature>(json);

        // Assert
        data.Should().NotBeNull();
        data!.Geometry.Should().BeNull();
        data!.BoundingBox.Should().BeNull();
        data!.Attributes.Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(CorrectData))]
    public void Deserialize_WhenCorrectJson_ShouldReturnCorrectData(string json)
    {
        // Arrange
        var serializer = new GeoJsonSerializer();

        // Act
        var data = serializer.Deserialize<IFeature>(json);

        // Assert
        data.Should().NotBeNull();
        data!.Geometry.Should().NotBeNull();
    }
}