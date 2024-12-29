using FluentResults;

using GoActive.Modules.Geo.Application.Shared.Dto;

using Mediator;

namespace GoActive.Modules.Geo.Application.Spot.Get;

public sealed class GetSpotQueryHandler : IQueryHandler<GetSpotQuery, Result<SpotDto>>
{
    public ValueTask<Result<SpotDto>> Handle(GetSpotQuery query, CancellationToken cancellationToken)
    {
        query.Deconstruct(out var spotId);

#pragma warning disable CS0618 // Type or member is obsolete

        var spot = Domain.SpotAggregate.Spot.InitialData.FirstOrDefault(x => x.Id == spotId);

#pragma warning restore CS0618 // Type or member is obsolete

#pragma warning disable S3358 // Ternary operators should not be nested

#pragma warning disable SA1118 // Parameter should not span multiple lines

        var result = spot is null
            ? Result.Fail($"Spot with id '{spotId}' was not found")
            : Result.Ok(new SpotDto
            {
                Id = spot.Id.Value,
                Title = spot.Title.Value,
                Location = new(
                                    spot.LocationPoint.Location.Latitude.Value,
                                    spot.LocationPoint.Location.Longitude.Value),
                Activities = [spot.Activities],
                Address = spot.Address is null ? null
                                    : new AddressDto
                                    {
                                        Country = spot.Address.Country,
                                        Region = spot.Address.Region,
                                        Settlement = spot.Address.Settlement,
                                        Street = spot.Address.Street,
                                    },
                Description = spot.Description,
            });
#pragma warning restore SA1118 // Parameter should not span multiple lines
#pragma warning restore S3358 // Ternary operators should not be nested

        return ValueTask.FromResult(result);
    }
}
