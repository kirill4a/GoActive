using FluentResults;
using MediatR;
using GoActive.Modules.Geo.Domain.SketchAggregate;
using GoActive.Modules.Geo.Domain.ValueObjects;

namespace GoActive.Modules.Geo.Application.Sketch.Create;

internal class CreateSketchCommandHandler : IRequestHandler<CreateSketchCommand, Result<Guid>>
{
    public Task<Result<Guid>> Handle(CreateSketchCommand command, CancellationToken cancellation)
    {
        // TODO: check the existence of the same sketch and return Result.Fail if it is so
        var latitude = new Latitude(command.Location.Latitude);
        var longitude = new Longitude(command.Location.Longitude);
        var location = new GeoLocation(latitude, longitude);
        Altitude? altitude = command.Altitude.HasValue ? new Altitude(command.Altitude.Value) : null;

        var newId = SketchId.FromValue(Guid.NewGuid());
        var title = Title.FromValue(command.Title);
        var locationPoint = altitude.HasValue
            ? GeoCoordinate.FromLocationWithAltitude(location, altitude.Value)
            : GeoCoordinate.FromLocation(location);

        var sketch = Domain.SketchAggregate.Sketch.Create(newId, title, locationPoint, command.ActivityTypes);

        // TODO: invoke save to database here (IUnitOfWork.CommitAsync())
        return Task.FromResult(Result.Ok(sketch.Id.Value));
    }
}