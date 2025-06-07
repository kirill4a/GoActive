using GoActive.Modules.Geo.Domain.SpotAggregate;
using GoActive.Shared.Domain.Enums;

namespace GoActive.Modules.Geo.Application.Spot.Search;

public interface ISpotSearcher
{
    /// <summary>
    /// Checks if the spot with the specified key exists in the storage.
    /// </summary>
    /// <param name="key">Spot key to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the spot exists, otherwise false.</returns>
    Task<bool> ExistsAsync(SpotKey key, CancellationToken cancellationToken);

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
