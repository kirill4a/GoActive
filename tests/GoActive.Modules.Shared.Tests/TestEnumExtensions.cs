using FluentAssertions;

using GoActive.Shared.Domain.Enums;
using GoActive.Shared.Domain.Extensions;

namespace GoActive.Modules.Shared.Tests;

public class TestEnumExtensions
{
    public static readonly TheoryData<ActivityTypes> AllActivityTypes = [];

    static TestEnumExtensions()
    {
        foreach (var item in Enum.GetValues<ActivityTypes>())
        {
            AllActivityTypes.Add(item);
        }
    }

    [Theory]
    [InlineData((ActivityTypes)(-1))]
    [InlineData((ActivityTypes)0)]
    [InlineData((ActivityTypes)101)]
    public void ActivityType_WhenFlagIsNotPresented_ShouldReturnsFalse(ActivityTypes activity)
    {
        // Act
        var isSuitable = activity.IsFlagSuitable();

        // Assert
        isSuitable.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(AllActivityTypes))]
    [InlineData(ActivityTypes.NordicSki | ActivityTypes.Workout)]
    public void ActivityType_WhenFlagIsPresented_ShouldReturnsTrue(ActivityTypes activity)
    {
        // Act
        var isSuitable = activity.IsFlagSuitable();

        // Assert
        isSuitable.Should().BeTrue();
    }
}
