using FluentResults;

using Mediator;

namespace GoActive.Modules.Geo.Application.Spot.Search;

public class SearchSpotsQueryHandler(ISpotSearcher spotSearcher) : IQueryHandler<SearchSpotsQuery, Result<SearchSpotResult[]>>
{
    public async ValueTask<Result<SearchSpotResult[]>> Handle(SearchSpotsQuery query, CancellationToken cancellationToken)
    {
        query.Deconstruct(out var queryString, out var activities);

        var searchedBySpot = await spotSearcher.SearchBySpot(queryString, activities, cancellationToken);
        var searchedByAddress = await spotSearcher.SearchByAddress(queryString, activities, cancellationToken);

        var searchResult = searchedBySpot.Union(searchedByAddress);
        return Result.Ok(searchResult.ToArray());
    }
}
