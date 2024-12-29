using FluentResults;

using Mediator;

namespace GoActive.Modules.Geo.Application.Spot.Search;

public class LocateSpotsQueryHandler : IQueryHandler<LocateSpotsQuery, Result<IReadOnlyCollection<SpotDto>>>
{
    public ValueTask<Result<IReadOnlyCollection<SpotDto>>> Handle(LocateSpotsQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
