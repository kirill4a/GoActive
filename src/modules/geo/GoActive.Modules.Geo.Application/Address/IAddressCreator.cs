namespace GoActive.Modules.Geo.Application.Address;

public interface IAddressCreator
{
    /// <summary>
    /// Creates an addresses in the storage.
    /// </summary>
    /// <param name="addresses">The collection of addresses.</param>
    /// <param name="cancellation">The cancellation token.</param>
    Task<int> CreateAddresses(IReadOnlyCollection<CreateAddressDto> addresses, CancellationToken cancellation);
}
