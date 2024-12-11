using FluentResults;
using Mediator;
using GoActive.Modules.Geo.Domain.SketchAggregate;
using GoActive.Modules.Geo.Domain.ValueObjects;

namespace GoActive.Modules.Geo.Application.Sketch.Create;

public sealed class CreateSketchCommandHandler : ICommandHandler<CreateSketchCommand, Result<Guid>>
{
    public ValueTask<Result<Guid>> Handle(CreateSketchCommand command, CancellationToken cancellation)
    {
        // TODO: check the existence of the same sketch and return Result.Fail if it is so
        var location = GeoLocation.FromLatLon(command.Location.Latitude, command.Location.Longitude);
        Altitude? altitude = command.Altitude.HasValue ? new Altitude(command.Altitude.Value) : null;

        var newId = SketchId.FromValue(Guid.NewGuid());
        var title = Title.FromValue(command.Title);
        var locationPoint = altitude.HasValue
            ? GeoCoordinate.FromLocationWithAltitude(location, altitude.Value)
            : GeoCoordinate.FromLocation(location);

        var sketch = Domain.SketchAggregate.Sketch.Create(newId, title, locationPoint, command.ActivityTypes);

        // TODO: invoke save to database here (IUnitOfWork.CommitAsync())
        return ValueTask.FromResult(Result.Ok(sketch.Id.Value));
    }
}
