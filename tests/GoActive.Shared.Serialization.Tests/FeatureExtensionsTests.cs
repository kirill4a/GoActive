using FluentAssertions;

using GoActive.Shared.Domain.Enums;
using GoActive.Shared.Serialization.NetTopologySuite;
using GoActive.Shared.Serialization.Tests.TestData;

using NetTopologySuite.Features;

namespace GoActive.Shared.Serialization.Tests;

public class FeatureExtensionsTests
{
    private static readonly FakeFeature _fakeFeature = new();

    public static TheoryData<IFeature?, string?> IncorrectArguments => new()
    {
        { null!, null! },
        { null!, "activities" },
        { _fakeFeature, null },
        { _fakeFeature, string.Empty },
        { _fakeFeature, "     " },
    };

    public static TheoryData<IFeature?, string?> EnumTypeMismatchData => new()
    {
        {
            new FakeFeature().WithAttribute("enumNamesArray", new string[]
                                                                {
                                                                    DateTimeKind.Unspecified.ToString(),
                                                                    DateTimeKind.Local.ToString(),
                                                                    DateTimeKind.Utc.ToString(),
                                                                }),
            "enumNamesArray"
        },
        {
            new FakeFeature().WithAttribute("enumNamesArray", new object[]
                                                                {
                                                                    DateTimeKind.Unspecified,
                                                                    DateTimeKind.Local,
                                                                    DateTimeKind.Utc,
                                                                }),
            "enumNamesArray"
        },
    };

    public static TheoryData<IFeature?, string?> UnexpectedData => new()
    {
        {
            new FakeFeature(),
            "anyName"
        },
        {
            new FakeFeature().WithAttribute("any name", "some name"),
            "any Name"
        },
        {
            new FakeFeature().WithAttribute("intArray", new int[] { 10, 20, 30, 40 }),
            "intArray"
        },
        {
            new FakeFeature().WithAttribute("enumArray", new DateTimeKind[] { DateTimeKind.Unspecified, DateTimeKind.Local, DateTimeKind.Utc }),
            "enumArray"
        },
    };

    [Theory]
    [MemberData(nameof(IncorrectArguments))]
    public void ExtractEnumValues_WhenIncorrectArguments_ShouldThrowArgumentException(IFeature? feature, string? attributeName)
    {
        // Act
        var act = () => feature!.ExtractEnumValues<ActivityTypes>(attributeName!);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [MemberData(nameof(EnumTypeMismatchData))]
    public void ExtractEnumValues_WhenEnumTypeMismatchData_ShouldThrowInvalidCastException(IFeature? feature, string? attributeName)
    {
        // Act
        var act = () => feature!.ExtractEnumValues<ActivityTypes>(attributeName!);

        // Assert
        act.Should().Throw<InvalidCastException>();
    }

    [Theory]
    [MemberData(nameof(UnexpectedData))]
    public void ExtractEnumValues_WhenUnexpectedData_ShouldReturnNull(IFeature feature, string attributeName)
    {
        // Act
        var enumValues = feature.ExtractEnumValues<ActivityTypes>(attributeName);

        // Assert
        enumValues.Should().BeNull();
    }

    [Fact]
    public void ExtractEnumValues_WhenCorrectFeature_ShouldReturnCorrectData()
    {
        // Arrange
        var attributeName = "activities";
        var feature = new FakeSpotFeature();
        var expectedActivities = FakeSpotFeature.Activities;

        // Act
        var activities = feature.ExtractEnumValues<ActivityTypes>(attributeName);

        // Assert
        activities.Should().NotBeNull();
        activities.Should().BeEquivalentTo(expectedActivities);
    }
}
