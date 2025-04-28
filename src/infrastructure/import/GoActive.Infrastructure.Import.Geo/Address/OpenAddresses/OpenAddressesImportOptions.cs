using System.ComponentModel.DataAnnotations;

using GoActive.Infrastructure.Import.Geo.Models;

using Microsoft.Extensions.Options;

namespace GoActive.Infrastructure.Import.Geo.Address.OpenAddresses;

/// <summary>
/// Options for importing addresses from <a href="https://openaddresses.io/">OpenAddresses</a>.
/// </summary>
public record OpenAddressesImportOptions : IImportOptions
{
    /// <summary>
    /// The batch import options.
    /// </summary>
    public required BatchImportOptions BatchOptions { get; init; }

    /// <summary>
    /// The country code for the addresses being imported.
    /// </summary>
    /// <remarks>
    /// Should be 2-letter alpha ISO code.
    /// See <a href="https://en.wikipedia.org/wiki/List_of_ISO_3166_country_codes">ISO 3166-1 alpha-2</a> for more information.
    /// </remarks>
    /// <remarks>
    /// See batches in <a href="https://batch.openaddresses.io/">open address sources</a>.
    /// </remarks>
    [Required]
    [StringLength(maximumLength: 2, MinimumLength = 2, ErrorMessage = "Country code must be 2 characters long (ISO 3166-1 alpha-2).")]
    public required string CountryCode { get; init; }
}

/// <summary>
/// Validator for <see cref="OpenAddressesImportOptions"/>.
/// </summary>
/// <remarks>
/// Uses for source generators to generate the validation code.
/// </remarks>
[OptionsValidator]
public partial class OpenAddressesImportOptionsValidator : IValidateOptions<OpenAddressesImportOptions>
{
}
