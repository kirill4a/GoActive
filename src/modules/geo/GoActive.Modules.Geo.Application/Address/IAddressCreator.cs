namespace GoActive.Modules.Geo.Application.Address;

public interface IAddressCreator : IDisposable
{
    /// <summary>
    /// Creates or updates an addresses in the storage.
    /// </summary>
    /// <param name="addresses">The collection of addresses.</param>
    /// <param name="cancellation">The cancellation token.</param>
    Task<int> UpsertAddresses(IReadOnlyCollection<CreateAddressDto> addresses, CancellationToken cancellation);
}
