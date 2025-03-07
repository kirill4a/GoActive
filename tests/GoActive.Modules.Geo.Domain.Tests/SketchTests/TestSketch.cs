using FluentAssertions;

using GoActive.Modules.Geo.Domain.SketchAggregate;
using GoActive.Modules.Geo.Domain.ValueObjects;
using GoActive.Shared.Domain.Enums;

namespace GoActive.Modules.Geo.Domain.Tests.SketchTests;

public class TestSketch
{
    public static readonly TheoryData<SketchId, Title, GeoCoordinate, IReadOnlyCollection<ActivityTypes>> WrongArguments = [];

    private static readonly SketchId Id = SketchId.FromValue(Guid.NewGuid());
    private static readonly Title Title = Title.FromValue("QWERTY");
    private static readonly GeoCoordinate LocationPoint = GeoCoordinate.FromLocation(GeoLocation.FromLatLon(57.11d, 37.08));
    private static readonly ActivityTypes Activity = ActivityTypes.NordicSki;

    static TestSketch()
    {
        WrongArguments.Add(default, Title, LocationPoint, [Activity]);
        WrongArguments.Add(Id, null!, LocationPoint, [Activity]);
        WrongArguments.Add(Id, Title, default, [Activity]);
        WrongArguments.Add(Id, Title, LocationPoint, default!);
    }

    [Theory]
    [MemberData(nameof(WrongArguments))]
    public void Create_FromWrongValues_ShouldThrowException(SketchId id,
                                                            Title title,
                                                            GeoCoordinate locationPoint,
                                                            IReadOnlyCollection<ActivityTypes> activityTypes)
    {
        // Act
        var function = () => Sketch.Create(id, title, locationPoint, activityTypes);

        // Assert
        function.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_FromValidValue_ShouldCreatedAndFilled()
    {
        // Act
        var expectedActivities = new ActivityTypes[] { Activity };
        var sketch = Sketch.Create(Id, Title, LocationPoint, expectedActivities);

        // Assert
        sketch.Should().NotBeNull();
        sketch.Id.Should().Be(Id);
        sketch.Title.Should().Be(Title);
        sketch.LocationPoint.Should().Be(LocationPoint);
        sketch.ActivityTypes.Should().BeEquivalentTo(expectedActivities);
        sketch.CreatedAt.Should().BeBefore(DateTime.UtcNow);
        sketch.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Create_AfterCreated_ShouldHaveDomainEvent()
    {
        // Act
        var sketch = Sketch.Create(Id, Title, LocationPoint, [Activity]);

        // Assert
        sketch.DomainEvents.Should().NotBeNullOrEmpty();
        sketch.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void TwoSketch_WithSameIds_ShouldBeEqual()
    {
        // Arrange
        var skiActivity = ActivityTypes.NordicSki;
        var workoutActivity = ActivityTypes.Workout;

        // Act
        var sketchSki = Sketch.Create(Id, Title, LocationPoint, [skiActivity]);
        var sketchWorkout = Sketch.Create(Id, Title, LocationPoint, [workoutActivity]);

        // Assert
        sketchSki.ActivityTypes.Should().NotBeEquivalentTo(sketchWorkout.ActivityTypes);

        sketchSki.Id.Should().Be(sketchWorkout.Id);
        sketchSki.Should().NotBeSameAs(sketchWorkout);
        sketchSki.Should().Be(sketchWorkout);
        (sketchSki == sketchWorkout).Should().BeTrue();
    }
}
