using GoActive.Modules.Geo.Domain.SpotAggregate.Events;
using GoActive.Modules.Geo.Domain.SketchAggregate;
using GoActive.Modules.Geo.Domain.ValueObjects;

using GoActive.Shared.Domain;
using GoActive.Shared.Domain.Enums;
using GoActive.Shared.Domain.Extensions;

namespace GoActive.Modules.Geo.Domain.SpotAggregate;

/// <summary>
/// Geographical spot, any specific place.
/// </summary>
public sealed class Spot : EntityBase<SpotId>
{
    private Spot(SpotId id,
                 Title title,
                 GeoCoordinate locationPoint,
                 ActivityTypes activities,
                 Address? address = null,
                 string? description = null)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(title);

        if (locationPoint == default)
            throw new ArgumentException($"Incorrect location for {nameof(Spot)}: {locationPoint}", nameof(locationPoint));

        if (!activities.IsFlagSuitable())
            throw new ArgumentException($"Incorrect activity type: {activities}", nameof(activities));

        Title = title;
        LocationPoint = locationPoint;
        Activities = activities;
        Address = address;
        Description = description;
    }

    public Title Title { get; }
    public GeoCoordinate LocationPoint { get; }
    public ActivityTypes Activities { get; }
    public Address? Address { get; }
    public string? Description { get; }
    public string? MetadataJson { get; }

    public static Spot Create(SpotId id,
                              Title title,
                              GeoCoordinate locationPoint,
                              ActivityTypes activityTypes,
                              Address? address = null,
                              string? description = null)
    {
        var spot = new Spot(id, title, locationPoint, activityTypes, address, description)
        {
            CreatedAt = DateTime.UtcNow,
        };

        spot.AddDomainEvent(new SpotCreatedDomainEvent(spot.Id));

        return spot;
    }

    public static Spot FromSketch(Sketch sketch) => Create(
        SpotId.FromValue(Guid.NewGuid()),
        sketch.Title,
        sketch.LocationPoint,
        sketch.ActivityTypes);

    /// <summary>
    /// Just test data. Will be removed later.
    /// </summary>
    [Obsolete("Debug data")]
#pragma warning disable SA1201 // Elements should appear in the correct order
    public static IReadOnlyCollection<Spot> InitialData => [

        Spot.Create(
            SpotId.FromValue(Guid.Parse("7ce1bbcb-a370-4a54-bf5d-03a1890c3cd6")),
            Title.FromValue("Kontiolahti biathlon stadium"),
            GeoCoordinate.FromLocation(GeoLocation.FromLatLon(62.72097, 29.81627)),
            ActivityTypes.NordicSki | ActivityTypes.Biathlon,
            Address.Create(
                country: "Finland",
                region: "Karelia",
                settlement: "Kontiolahti"),
            "The best nordic ski and biathlon area in eastern Finland"),

        Spot.Create(
            SpotId.FromValue(Guid.Parse("2bafb2ec-09fb-4165-9697-7d0cf5e6c6b5")),
            Title.FromValue("Hochfilzen biathlon stadium"),
            GeoCoordinate.FromLocation(GeoLocation.FromLatLon(47.475781, 12.637753)),
            ActivityTypes.NordicSki | ActivityTypes.Biathlon,
            Address.Create(
                country: "Austria",
                settlement: "Hochfilzen"),
            "The best nordic ski and biathlon area in Austria"),

        Spot.Create(
            SpotId.FromValue(Guid.Parse("16015d87-9401-486b-bd3b-cba4cf1cb5c6")),
            Title.FromValue("Ancee - Le Grand Bornand biathlon stadium"),
            GeoCoordinate.FromLocation(GeoLocation.FromLatLon(45.940777944299896, 6.431652646031023)),
            ActivityTypes.NordicSki | ActivityTypes.Biathlon,
            Address.Create(
                country: "France",
                region: "Annecy",
                settlement: "Le Grand Bornand"),
            "Stade de Biathlon Sylvie Becaert"),

    ];
#pragma warning restore SA1201 // Elements should appear in the correct order

}
