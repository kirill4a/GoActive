using FluentResults;

using Mediator;

namespace GoActive.Modules.Geo.Application.Spot.Locate;

public class LocateSpotsQueryHandler : IQueryHandler<LocateSpotsQuery, Result<LocateSpotResult[]>>
{
    public ValueTask<Result<LocateSpotResult[]>> Handle(LocateSpotsQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
