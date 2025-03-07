using GoActive.Modules.Geo.Domain.SpotAggregate.Events;
using GoActive.Modules.Geo.Domain.SketchAggregate;
using GoActive.Modules.Geo.Domain.ValueObjects;

using GoActive.Shared.Domain;
using GoActive.Shared.Domain.Enums;

namespace GoActive.Modules.Geo.Domain.SpotAggregate;

/// <summary>
/// Geographical spot, any specific place.
/// </summary>
public sealed class Spot : EntityBase<SpotId>
{
    private Spot(SpotId id,
                 Title title,
                 GeoCoordinate locationPoint,
                 IReadOnlyCollection<ActivityTypes> activities,
                 Address? address = null,
                 string? description = null)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(activities);

        if (locationPoint == default)
            throw new ArgumentException($"Incorrect location for {nameof(Spot)}: {locationPoint}", nameof(locationPoint));

        Title = title;
        LocationPoint = locationPoint;
        Activities = activities;
        Address = address;
        Description = description;
    }

    public Title Title { get; }
    public GeoCoordinate LocationPoint { get; }
    public IReadOnlyCollection<ActivityTypes> Activities { get; }
    public Address? Address { get; }
    public string? Description { get; }
    public string? MetadataJson { get; }

    public static Spot Create(SpotId id,
                              Title title,
                              GeoCoordinate locationPoint,
                              IReadOnlyCollection<ActivityTypes> activityTypes,
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
            [ActivityTypes.NordicSki, ActivityTypes.Biathlon],
            Address.Create(
                country: "Finland",
                region: "Karelia",
                settlement: "Kontiolahti"),
            "The best nordic ski and biathlon area in eastern Finland"),

        Spot.Create(
            SpotId.FromValue(Guid.Parse("2bafb2ec-09fb-4165-9697-7d0cf5e6c6b5")),
            Title.FromValue("Hochfilzen biathlon stadium"),
            GeoCoordinate.FromLocation(GeoLocation.FromLatLon(47.475781, 12.637753)),
            [ActivityTypes.Biathlon],
            Address.Create(
                country: "Austria",
                settlement: "Hochfilzen"),
            "The best nordic ski and biathlon area in Austria"),

        Spot.Create(
            SpotId.FromValue(Guid.Parse("16015d87-9401-486b-bd3b-cba4cf1cb5c6")),
            Title.FromValue("Ancee - Le Grand Bornand biathlon stadium"),
            GeoCoordinate.FromLocation(GeoLocation.FromLatLon(45.940777944299896, 6.431652646031023)),
            [ActivityTypes.NordicSki, ActivityTypes.Biathlon, ActivityTypes.Workout, ActivityTypes.RollerSki],
            Address.Create(
                country: "France",
                region: "Annecy",
                settlement: "Le Grand Bornand"),
            "Stade de Biathlon Sylvie Becaert"),

            Create(
            SpotId.FromValue(Guid.Parse("a185ce91-92ba-4750-a43e-cea048c1f8aa")),
            Title.FromValue("Oberhof biathlon stadium"),
            GeoCoordinate.FromLocation(GeoLocation.FromLatLon(50.707222, 10.694722)),
            [ActivityTypes.NordicSki, ActivityTypes.Biathlon, ActivityTypes.RollerSki],
            Address.Create(
                country: "Germany",
                region: "Thüringen forest",
                settlement: "Oberhof"),
            "Oberhof is known as a German winter sports centre, especially for biathlon and sliding sports. The new LOTTO Thüringen ARENA am Rennsteig will pack more than 100,000 biathlon fans for the BMW IBU World Championships Biathlon in February 2023."),

            Create(
            SpotId.FromValue(Guid.Parse("0bf70768-5ea0-4b5a-b362-eb4bbdb2ef36")),
            Title.FromValue("Ruhpolding Venue Arena - Chiemgau Arena - f"),
            GeoCoordinate.FromLocationWithAltitude(GeoLocation.FromLatLon(47.715, 12.646111), new(713)),
            [ActivityTypes.NordicSki, ActivityTypes.Biathlon, ActivityTypes.RollerSki],
            Address.Create(
                country: "Germany",
                region: "Chiemgau",
                settlement: "Ruhpolding"),
            "Nestled in the Bavarian Alps, the Chiemgau Arena provides a spectacular atmosphere for enthusiastic fans and calm conditions at the shooting range for the athletes."),

            Spot.Create(
            SpotId.FromValue(Guid.Parse("be969cb6-9f06-420c-8597-a091b0a80c07")),
            Title.FromValue("Antholz-Anterselva biathlon stadium - f"),
            GeoCoordinate.FromLocation(GeoLocation.FromLatLon(46.884167, 12.153611)),
            [ActivityTypes.NordicSki, ActivityTypes.Biathlon],
            Address.Create(
                country: "Italy",
                region: "Südtirol",
                settlement: "Antholz"),
            "The Südtirol Arena, perfectly dropped below a stunning snow-capped peak, tops out the climb up the valley. Set at 1600-meters high, the Arena and surrounding tracks are almost guaranteed to look like a winter wonderland each January."),

            Create(
            SpotId.FromValue(Guid.Parse("c1f217ea-49f1-4f89-89c3-9df38c15015e")),
            Title.FromValue("Lenzerheide - Roland Arena - World 2025 - f"),
            GeoCoordinate.FromLocationWithAltitude(GeoLocation.FromLatLon(46.692222, 9.558333), new(1400)),
            [ActivityTypes.NordicSki, ActivityTypes.Biathlon, ActivityTypes.RollerSki],
            Address.Create(
                country: "Switzerland",
                region: "Graubünden",
                settlement: "Lenzerheide"),
            "The Roland Arena is located in Lantsch/Lenz, about five kilometres from the village centre of Lenzerheide. Thanks to the multifunctional building architecture, the arena has developed into a well-known sports hotspot and is a meeting place for elite and junior athletes, leisure athletes, clubs and sports associations."),

    ];
#pragma warning restore SA1201 // Elements should appear in the correct order

}
