using GoActive.Modules.Geo.Domain.SpotAggregate;
using GoActive.Modules.Geo.Domain.ValueObjects;

using Mediator;

using DomainSpot = GoActive.Modules.Geo.Domain.SpotAggregate.Spot;

namespace GoActive.Modules.Geo.Application.Spot.Create;

public class CreateSpotsCommandHandler(ISpotCreator spotCreator) : ICommandHandler<CreateSpotsCommand, int>
{
    public async ValueTask<int> Handle(CreateSpotsCommand command, CancellationToken cancellationToken)
    {
        await Task.Yield(); // Simulate async operation
        _ = spotCreator;
        _ = command.SpotDtos.Select(CreateSpot).ToList();

#pragma warning disable S125 // Sections of code should not be commented out
        /*
        return await spotCreator.CreateSpotsAsync(spots, cancellationToken);
        */
#pragma warning restore S125 // Sections of code should not be commented out

        throw new NotImplementedException();

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
