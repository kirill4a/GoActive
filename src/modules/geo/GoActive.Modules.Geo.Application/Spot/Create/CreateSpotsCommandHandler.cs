using GoActive.Modules.Geo.Application.Shared.Storage;
using GoActive.Modules.Geo.Domain.SpotAggregate;
using GoActive.Modules.Geo.Domain.ValueObjects;

using Mediator;

using DomainSpot = GoActive.Modules.Geo.Domain.SpotAggregate.Spot;

namespace GoActive.Modules.Geo.Application.Spot.Create;

public class CreateSpotsCommandHandler(ISpotCreator spotCreator, IUnitOfWork unitOfWork) : ICommandHandler<CreateSpotsCommand, int>
{
    public async ValueTask<int> Handle(CreateSpotsCommand command, CancellationToken cancellationToken)
    {
        var spots = command.SpotDtos.Select(CreateSpot).ToList();
        spotCreator.CreateSpots(spots);

        return await unitOfWork.CommitAsync(cancellationToken);
    }

    private static DomainSpot CreateSpot(CreateSpotDto spotDto)
    {
        var spotLocation = spotDto.Altitude.HasValue
                        ? GeoCoordinate.FromLocationWithAltitude(GeoLocation.FromLatLon(spotDto.Latitude, spotDto.Longitude),
                                                                 new(spotDto.Altitude.Value))
                        : GeoCoordinate.FromLocation(GeoLocation.FromLatLon(spotDto.Latitude, spotDto.Longitude));

        return DomainSpot.Create(
            SpotId.FromValue(Guid.NewGuid()),
            Title.FromValue(spotDto.Title),
            spotLocation,
            spotDto.Activities,
            spotDto.AddressId.HasValue ? AddressId.FromValue(spotDto.AddressId.Value) : null,
            address: null,
            spotDto.Description);

    }
}
