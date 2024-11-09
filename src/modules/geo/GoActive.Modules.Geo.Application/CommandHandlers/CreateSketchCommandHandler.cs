using FluentResults;
using MediatR;
using GoActive.Modules.Geo.Application.Commands;
using GoActive.Modules.Geo.Domain.GeoDataAggregate;
using GoActive.Modules.Geo.Domain.ValueObjects;

namespace GoActive.Modules.Geo.Application.CommandHandlers;

internal class CreateSketchCommandHandler : IRequestHandler<CreateSketchCommand, Result<Guid>>
{
    public Task<Result<Guid>> Handle(CreateSketchCommand command, CancellationToken cancellation)
    {
        try
        {
            var newId = GeoDataId.FromValue(Guid.NewGuid());
            var latitude = new Latitude(command.Location.Latitude);
            var longitude = new Longitude(command.Location.Longitude);

            var geoData = new GeoData(newId, Title.FromValue(command.Title))
            {
                CreatedAt = DateTime.UtcNow,
                Center = new GeoCoordinate(Location: new(latitude, longitude), Altitude: default)
            };

            //TODO: invoke save to database here

            return Task.FromResult(Result.Ok(geoData.Id.Value));
        }
        // TODO: catch the specific business excception
        catch (Exception ex)
        {

            // TODO: log the exception
            return Task.FromResult(Result.Fail<Guid>(ex.Message));
        }
    }
}
