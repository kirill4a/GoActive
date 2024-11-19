using GoActive.Modules.Geo.Domain.SketchAggregate.Events;
using GoActive.Modules.Geo.Domain.ValueObjects;
using GoActive.Shared.Domain;
using GoActive.Shared.Domain.Enums;
using GoActive.Shared.Domain.Extensions;

namespace GoActive.Modules.Geo.Domain.SketchAggregate;

public class Sketch : EntityBase<SketchId>
{
    private Sketch(SketchId id, Title title, GeoCoordinate locationPoint, ActivityTypes activityTypes) : base(id)
    {
        ArgumentNullException.ThrowIfNull(title);

        // Assuming that there is shouldn't be sport object at the sea point with coordinates 0:0 and altitude 0 (sea level)
        if (locationPoint == default)
            throw new ArgumentException($"Incorrect location for sketch: {locationPoint}", nameof(locationPoint));

        if (!activityTypes.IsFlagSuitable())
            throw new ArgumentException($"Incorrect activity type: {activityTypes}", nameof(activityTypes));

        Title = title;
        LocationPoint = locationPoint;
        ActivityTypes = activityTypes;
    }

    public static Sketch Create(SketchId id, Title title, GeoCoordinate locationPoint, ActivityTypes activityTypes)
    {
        var sketch = new Sketch(id, title, locationPoint, activityTypes)
        {
            CreatedAt = DateTime.UtcNow
        };

        sketch.AddDomainEvent(new SketchCreatedDomainEvent(sketch.Id));

        return sketch;
    }

    public Title Title { get; }
    public GeoCoordinate LocationPoint { get; }
    public ActivityTypes ActivityTypes { get; }
}
