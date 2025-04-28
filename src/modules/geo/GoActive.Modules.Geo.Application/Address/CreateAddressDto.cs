using NetTopologySuite.Geometries;

namespace GoActive.Modules.Geo.Application.Address;

public sealed record CreateAddressDto
{
    /// <summary>
    /// The source of address.
    /// </summary>
    public required AddressSource Source { get; init; }

    /// <summary>
    /// Address identifier in the external source.
    /// </summary>
    public required string ExternalId { get; init; }

    /// <summary>
    /// The 2-alpha country code.
    /// </summary>
    /// <remarks>According to <a href="https://www.iso.org/iso-3166-country-codes.html">ISO 3166</a>.</remarks>
    public required string CountryCode { get; init; }

    /// <summary>
    /// The region of the country.
    /// </summary>
    public string? Region { get; init; }

    /// <summary>
    /// The district name.
    /// </summary>
    public string? District { get; init; }

    /// <summary>
    /// The settlement (city, village, etc) name.
    /// </summary>
    public string? Settlement { get; init; }

    /// <summary>
    /// The street name.
    /// </summary>
    public string? Street { get; init; }

    /// <summary>
    /// The house number.
    /// </summary>
    public string? Building { get; init; }

    /// <summary>
    /// The postal code.
    /// </summary>
    public string? PostalCode { get; init; }

    /// <summary>
    /// The hash (checksum) of the address valuable fields (country, district, region etc).
    /// </summary>
    public required string Hash { get; init; }

    /// <summary>
    /// Address location point.
    /// </summary>
    public required Point Location { get; init; }
}
