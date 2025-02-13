using FluentResults;

using GoActive.Modules.Geo.Application.Shared.Dto;

using Mediator;
using GoActive.Modules.Geo.Domain.SketchAggregate;
using GoActive.Modules.Geo.Domain.ValueObjects;
using GoActive.Shared.Domain.Enums;

namespace GoActive.Modules.Geo.Application.Sketch.Create;

public sealed class CreateSketchCommandHandler : ICommandHandler<CreateSketchCommand, Result<SketchDto>>
{
    public ValueTask<Result<SketchDto>> Handle(CreateSketchCommand command, CancellationToken cancellation)
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

        var sketchDto = new SketchDto(
            sketch.Id.Value,
            sketch.Title.Value,
            new GeoLocationDto(
                sketch.LocationPoint.Location.Latitude.Value,
                sketch.LocationPoint.Location.Longitude.Value),
            sketch.ActivityTypes);

        // TODO: invoke save to database here (IUnitOfWork.CommitAsync())
        return ValueTask.FromResult(Result.Ok(sketchDto));
    }
}
