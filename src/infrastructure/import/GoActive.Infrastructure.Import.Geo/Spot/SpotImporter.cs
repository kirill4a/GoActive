using System.Diagnostics.CodeAnalysis;

using GoActive.Infrastructure.Import.Geo.Models;
using GoActive.Modules.Geo.Application.Spot.Create;
using GoActive.Modules.Geo.Domain.SpotAggregate;
using GoActive.Modules.Geo.Domain.ValueObjects;
using GoActive.Shared.Domain.Enums;
using GoActive.Shared.Serialization.NetTopologySuite;

using Microsoft.Extensions.Logging;

using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

using static GoActive.Infrastructure.Import.Geo.Spot.SpotConstants;

using DomainSpot = GoActive.Modules.Geo.Domain.SpotAggregate.Spot;

namespace GoActive.Infrastructure.Import.Geo.Spot;

internal sealed class SpotImporter(ISpotCreator spotCreator, ILogger<SpotImporter> logger) : FeaturesImporter<BatchImportOptions>(logger)
{
    protected override HashSet<string> RequiedFields =>
    [
        Fields.Id,
        Fields.Title,
        Fields.Activities,
    ];

    protected override BatchImportOptions? GetBatchOptions(BatchImportOptions options) => options;

    protected override async ValueTask<ImportResult> HandleFeatures(IReadOnlyCollection<IFeature> features,
                                                                    BatchImportOptions options,
                                                                    CancellationToken cancellationToken)
    {
        var errorCount = 0;
        var spots = new List<DomainSpot>(features.Count);

        foreach (var feature in features)
        {
            if (!TryMap(feature, out var spot))
            {
                errorCount++;
                continue;
            }

            spots.Add(spot);
        }

        if (spots.Count == 0)
        {
            return new(features.Count, 0, errorCount);
        }

        var successCount = await spotCreator.BulkInsertSpotsAsync(spots, cancellationToken);

        return new(features.Count, successCount, errorCount);
    }

    private static float ToFloatWithCheck(double input)
    {
        if (input >= float.MinValue && input <= float.MaxValue)
        {
            return (float)input;
        }

        throw new OverflowException($"Value '{input}' is out of float range.");
    }

    private static GeoCoordinate ExtractLocationPoint(Point point)
    {
        var location = GeoLocation.FromLatLon(point.X, point.Y);

        return point.Z is Coordinate.NullOrdinate
            ? GeoCoordinate.FromLocation(location)
            : GeoCoordinate.FromLocationWithAltitude(location, new(ToFloatWithCheck(point.Z)));
    }

    private static AddressId? ExtractAddressId(IFeature feature)
    {
        var addressIdString = feature.Attributes.GetOptionalValue(Fields.AddressId)?.ToString();
        if (string.IsNullOrWhiteSpace(addressIdString))
        {
            return null;
        }

        return AddressId.FromValue(Guid.Parse(addressIdString));
    }

    private bool TryMap(IFeature feature, [NotNullWhen(true)] out DomainSpot? spot)
    {
        spot = null;

        if (!ValidateRequiredFields(feature) ||
            !ValidateRequiredValue(feature, Fields.Id, out var id) ||
            !ValidateRequiredValue(feature, Fields.Title, out var title) ||
            !ValidateRequiredValue(feature, Fields.Activities, out _))
        {
            return false;
        }

        if (!Guid.TryParse(id, out var spotId))
        {
            logger.LogError("Incorrect Spot ID (should be Guid) '{Id}' in feature: {Feature}", id, feature);
            return false;
        }

        if (feature.Geometry is not Point point)
        {
            logger.LogError("Expected a point geometry, but got '{GeometryType}'", feature.Geometry.GeometryType);
            return false;
        }

        if (!TryExtractActivities(feature, out var activities))
        {
            logger.LogError("No valid activities were found in the feature: {Feature}", feature);
            return false;
        }

        var addressId = ExtractAddressId(feature);
        var locationPoint = ExtractLocationPoint(point);

        spot = DomainSpot.Create(
                            id: SpotId.FromValue(spotId),
                            title: Title.FromValue(title),
                            locationPoint: locationPoint,
                            activities: activities,
                            addressId: addressId,
                            address: null,
                            description: feature.Attributes.GetOptionalValue(Fields.Description)?.ToString());
        return true;
    }

    private bool TryExtractActivities(IFeature feature, [NotNullWhen(true)] out IReadOnlyCollection<ActivityType>? activities)
    {
        try
        {
            activities = feature.ExtractEnumValues<ActivityType>(Fields.Activities);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to extract activities from the feature: {Feature}", feature);
            activities = null;
            return false;
        }

        return activities is not null && activities.Count > 0;
    }
}
