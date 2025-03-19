using DomainSpot = GoActive.Modules.Geo.Domain.SpotAggregate.Spot;

namespace GoActive.Modules.Geo.Application.Spot.Create;

public interface ISpotCreator
{
    /// <summary>
    /// Creates a new spot in the storage.
    /// </summary>
    /// <param name="spots">The collection of domain spots.</param>
    void CreateSpots(IReadOnlyCollection<DomainSpot> spots);
}
