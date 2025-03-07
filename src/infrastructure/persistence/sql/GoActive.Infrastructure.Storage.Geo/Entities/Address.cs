using GoActive.Infrastructure.Storage.Geo.Enums;

using NetTopologySuite.Geometries;

namespace GoActive.Infrastructure.Storage.Geo.Entities;

/// <summary>
/// Database entity for Address.
/// </summary>
internal class Address : Entity
{
  /// <summary>
  /// Unique address identifier.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// The source of address.
  /// </summary>
  public required AddressSource Source { get; set; }

  /// <summary>
  /// Address identifier in the external source.
  /// </summary>
  public required string ExternalId { get; set; }

  /// <summary>
  /// The 2-alpha country code.
  /// </summary>
  /// <remarks>According to <a href="https://www.iso.org/iso-3166-country-codes.html">ISO 3166</a>.</remarks>
  public required string Country { get; set; }

  /// <summary>
  /// The region of the country.
  /// </summary>
  public string? Region { get; set; }

  /// <summary>
  /// The district name.
  /// </summary>
  public string? District { get; set; }

  /// <summary>
  /// The settlement (city, village, etc) name.
  /// </summary>
  public string? Settlement { get; set; }

  /// <summary>
  /// The street name.
  /// </summary>
  public string? Street { get; set; }

  /// <summary>
  /// The house number.
  /// </summary>
  public string? Building { get; set; }

  /// <summary>
  /// The postal code.
  /// </summary>
  public string? PostCode { get; set; }

  /// <summary>
  /// The hash (checksum) of the address valuable fields (country, district, region etc).
  /// </summary>
  public required string Hash { get; set; }

  /// <summary>
  /// Address location point.
  /// </summary>
  public required Point Location { get; set; }

  /// <summary>
  /// Flag shows the address is active and actual.
  /// </summary>
  public bool IsActive { get; set; }
}
