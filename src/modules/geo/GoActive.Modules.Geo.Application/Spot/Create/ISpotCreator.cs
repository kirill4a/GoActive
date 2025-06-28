using DomainSpot = GoActive.Modules.Geo.Domain.SpotAggregate.Spot;

namespace GoActive.Modules.Geo.Application.Spot.Create;

public interface ISpotCreator
{
    /// <summary>
    /// Asynchronously bulk inserts a collection of spots into the storage.
    /// </summary>
    /// <param name="spots">The collection of domain spots to insert.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
    /// returns>The number of spots successfully inserted.</returns>
    Task<int> BulkInsertSpotsAsync(IReadOnlyCollection<DomainSpot> spots, CancellationToken cancellationToken);
}
