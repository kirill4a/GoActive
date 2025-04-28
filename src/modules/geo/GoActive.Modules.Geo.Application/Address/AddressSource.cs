namespace GoActive.Modules.Geo.Application.Address;

/// <summary>
/// The source of address data.
/// </summary>
/// <remarks>Move to the level of address importers later.</remarks>
public enum AddressSource
{

    /// <summary>
    /// The default value if not set.
    /// </summary>
    None = 0,

    /// <summary>
    /// The address data is from OpenAddresses.
    /// </summary>
    /// <remarks>
    /// <a href="https://openaddresses.io">openaddresses.io</a>
    /// </remarks>
    OpenAddresses,
}
