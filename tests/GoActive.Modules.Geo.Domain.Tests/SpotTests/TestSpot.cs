using FluentAssertions;

using GoActive.Modules.Geo.Domain.SpotAggregate;
using GoActive.Modules.Geo.Domain.ValueObjects;
using GoActive.Shared.Domain.Enums;

namespace GoActive.Modules.Geo.Domain.Tests.SpotTests;

public class TestSpot
{
    public static readonly TheoryData<SpotId, Title, GeoCoordinate, IReadOnlyCollection<ActivityTypes>, AddressId, Address, string> WrongArguments = [];

    private static readonly SpotId Id = SpotId.FromValue(Guid.NewGuid());
    private static readonly Title Title = Title.FromValue("Test spot title");
    private static readonly GeoCoordinate LocationPoint = GeoCoordinate.FromLocation(GeoLocation.FromLatLon(57.11d, 37.08));
    private static readonly ActivityTypes Activity = ActivityTypes.NordicSki;
    private static readonly AddressId AddressId = AddressId.FromValue(Guid.NewGuid());
    private static readonly Address Address = Address.Create("AQ");
    private static readonly string Description = "Any description";

    static TestSpot()
    {
        WrongArguments.Add(default, Title, LocationPoint, [Activity], AddressId, Address, Description);
        WrongArguments.Add(Id, null!, LocationPoint, [Activity], AddressId, Address, Description);
        WrongArguments.Add(Id, Title, default, [Activity], AddressId, Address, Description);
        WrongArguments.Add(Id, Title, LocationPoint, default!, AddressId, Address, Description);
        WrongArguments.Add(Id, Title, LocationPoint, [], AddressId, Address, Description);
    }

    [Theory]
    [MemberData(nameof(WrongArguments))]
    public void Create_FromWrongValues_ShouldThrowException(SpotId id,
                                                            Title title,
                                                            GeoCoordinate locationPoint,
                                                            IReadOnlyCollection<ActivityTypes> activityTypes,
                                                            AddressId addressId,
                                                            Address address,
                                                            string description)
    {
        // Act
        var function = () => Spot.Create(id, title, locationPoint, activityTypes, addressId, address, description);

        // Assert
        function.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_FromValidValue_ShouldCreatedAndFilled()
    {
        // Act
        var spot = Spot.Create(Id, Title, LocationPoint, [Activity], AddressId, Address, Description);

        // Assert
        spot.Should().NotBeNull();
        spot.Id.Should().Be(Id);
        spot.Title.Should().Be(Title);
        spot.LocationPoint.Should().Be(LocationPoint);
        spot.Activities.Should().BeEquivalentTo([Activity]);
        spot.Description.Should().Be(Description);
        spot.CreatedAt.Should().BeBefore(DateTime.UtcNow);
        spot.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Create_AfterCreated_ShouldHaveDomainEvent()
    {
        // Act
        var spot = Spot.Create(Id, Title, LocationPoint, [Activity]);

        // Assert
        spot.DomainEvents.Should().NotBeNullOrEmpty();
        spot.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void TwoSpot_WithSameIds_ShouldBeEqual()
    {
        // Arrange
        var skiActivity = ActivityTypes.NordicSki;
        var workoutActivity = ActivityTypes.Workout;

        // Act
        var spotSki = Spot.Create(Id, Title, LocationPoint, [skiActivity]);
        var spotWorkout = Spot.Create(Id, Title, LocationPoint, [workoutActivity]);

        // Assert
        spotSki.Activities.Should().NotBeEquivalentTo(spotWorkout.Activities);

        spotSki.Id.Should().Be(spotWorkout.Id);
        spotSki.Should().NotBeSameAs(spotWorkout);
        spotSki.Should().Be(spotWorkout);
        (spotSki == spotWorkout).Should().BeTrue();
    }
}
