using GoActive.Shared.Domain.Enums;

namespace GoActive.Modules.Geo.Application.Spot.Search;

public interface ISpotSearcher
{
    /// <summary>
    /// Performs the search for spots based on spot attributies.
    /// </summary>
    /// <param name="queryString">Text to search in the spot attributies.</param>
    /// <param name="activities">Activity types to filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The collection of spots satisfying query conditions.</returns>
    Task<IReadOnlyCollection<SearchSpotResult>> SearchBySpot(string queryString,
                                                             IReadOnlyCollection<ActivityType> activities,
                                                             CancellationToken cancellationToken);

    /// <summary>
    /// Performs the search for spots based on spot address.
    /// </summary>
    /// <param name="queryString">Text to search in the spot address attributies</param>
    /// <param name="activities">Activity types to filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The collection of spots satisfying query conditions.</returns>
    Task<IReadOnlyCollection<SearchSpotResult>> SearchByAddress(string queryString,
                                                                IReadOnlyCollection<ActivityType> activities,
                                                                CancellationToken cancellationToken);
}
