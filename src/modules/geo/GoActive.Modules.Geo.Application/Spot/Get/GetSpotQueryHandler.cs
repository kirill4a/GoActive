using FluentResults;

using GoActive.Modules.Geo.Application.Spot.Search;

using Mediator;

namespace GoActive.Modules.Geo.Application.Spot.Get;

public sealed class GetSpotQueryHandler(ISpotSearcher spotSearcher) : IQueryHandler<GetSpotQuery, Result<GetSpotResult>>
{
    private readonly ISpotSearcher _spotSearcher = spotSearcher;

    public async ValueTask<Result<GetSpotResult>> Handle(GetSpotQuery query, CancellationToken cancellationToken)
    {
        var spotResult = await _spotSearcher.GetAsync(query.SpotId, cancellationToken);
        return
         spotResult is null
            ? Result.Fail($"Spot with id '{query.SpotId}' was not found")
            : Result.Ok(spotResult);
    }
}
