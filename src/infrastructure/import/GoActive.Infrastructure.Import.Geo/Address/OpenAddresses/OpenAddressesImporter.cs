using System.Diagnostics.CodeAnalysis;

using GoActive.Infrastructure.Import.Geo.Models;
using GoActive.Modules.Geo.Application.Address;

using Microsoft.Extensions.Logging;

using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

using static GoActive.Infrastructure.Import.Geo.Address.AddressConstants;

namespace GoActive.Infrastructure.Import.Geo.Address.OpenAddresses;

internal sealed class OpenAddressesImporter(IAddressCreator addressCreator, ILogger<OpenAddressesImporter> logger)
    : FeaturesImporter<OpenAddressesImportOptions>(logger)
{
    protected override HashSet<string> RequiedFields =>
    [
        OpenAddressFields.Id,
        OpenAddressFields.Hash,
    ];

    protected override BatchImportOptions? GetBatchOptions(OpenAddressesImportOptions options) => options.BatchOptions;

    protected override async ValueTask<ImportResult> HandleFeatures(IReadOnlyCollection<IFeature> features,
                                                                    OpenAddressesImportOptions options,
                                                                    CancellationToken cancellation)
    {
        if (!ValidateCountryCode(options.CountryCode))
        {
            return new(features.Count, 0, features.Count);
        }

        var errorCount = 0;
        var dtos = new List<CreateAddressDto>(features.Count);

        foreach (var feature in features)
        {
            if (!TryMap(feature, options.CountryCode, out var dto))
            {
                errorCount++;
                continue;
            }

            dtos.Add(dto);
        }

        if (dtos.Count == 0)
        {
            return new(features.Count, 0, errorCount);
        }

        var successCount = await addressCreator.CreateAddresses(dtos, cancellation);

        return new(features.Count, successCount, errorCount);
    }

    private bool TryMap(IFeature feature, string countryCode, [NotNullWhen(true)] out CreateAddressDto? addressDto)
    {
        addressDto = null;

        if (!ValidateRequiredFields(feature) ||
            !ValidateRequiredValue(feature, OpenAddressFields.Id, out var addressId) ||
            !ValidateRequiredValue(feature, OpenAddressFields.Hash, out var hash))
        {
            return false;
        }

        if (feature.Geometry is not Point point)
        {
            logger.LogError("Expected a point geometry, but got '{GeometryType}'. Address id: '{AddressId}'", feature.Geometry.GeometryType, addressId);
            return false;
        }

        addressDto = new()
        {
            Source = AddressSource.OpenAddresses,
            ExternalId = addressId,
            CountryCode = countryCode,

            Region = feature.Attributes.GetOptionalValue(OpenAddressFields.Region)?.ToString(),
            District = feature.Attributes.GetOptionalValue(OpenAddressFields.District)?.ToString(),
            Settlement = feature.Attributes.GetOptionalValue(OpenAddressFields.City)?.ToString(),
            Street = feature.Attributes.GetOptionalValue(OpenAddressFields.Street)?.ToString(),
            Building = feature.Attributes.GetOptionalValue(OpenAddressFields.Number)?.ToString(),
            PostalCode = feature.Attributes.GetOptionalValue(OpenAddressFields.Postcode)?.ToString(),

            Hash = hash,
            Location = point,
        };
        return true;
    }

    private bool ValidateCountryCode(string countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
        {
            logger.LogError("Country code shouldn't be empty or whitespaces.");
            return false;
        }

        if (countryCode.Length != 2)
        {
            logger.LogError("Country code should be 2 characters long.");
            return false;
        }

        return true;
    }

}
