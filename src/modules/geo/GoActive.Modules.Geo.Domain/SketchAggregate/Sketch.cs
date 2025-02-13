using GoActive.Modules.Geo.Domain.SketchAggregate.Events;
using GoActive.Modules.Geo.Domain.ValueObjects;
using GoActive.Shared.Domain;
using GoActive.Shared.Domain.Enums;

namespace GoActive.Modules.Geo.Domain.SketchAggregate;

/// <summary>
/// The sketch (draft) of fitness object.
/// </summary>
public sealed class Sketch : EntityBase<SketchId>
{
    private Sketch(SketchId id, Title title, GeoCoordinate locationPoint, IReadOnlyCollection<ActivityTypes> activityTypes)
        : base(id)
    {
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(activityTypes);

        // Assuming that there is shouldn't be sport object at the sea point with coordinates 0:0 and altitude 0 (sea level)
        if (locationPoint == default)
            throw new ArgumentException($"Incorrect location for {nameof(Sketch)}: {locationPoint}", nameof(locationPoint));

        Title = title;
        LocationPoint = locationPoint;
        ActivityTypes = activityTypes;
    }

    public Title Title { get; }
    public GeoCoordinate LocationPoint { get; }
    public IReadOnlyCollection<ActivityTypes> ActivityTypes { get; }

    public static Sketch Create(SketchId id, Title title, GeoCoordinate locationPoint, IReadOnlyCollection<ActivityTypes> activityTypes)
    {
        var sketch = new Sketch(id, title, locationPoint, activityTypes)
        {
            CreatedAt = DateTime.UtcNow,
        };

        sketch.AddDomainEvent(new SketchCreatedDomainEvent(sketch.Id));

        return sketch;
    }

    public void Apply() => AddDomainEvent(new SketchAppliedDomainEvent(this));
}
