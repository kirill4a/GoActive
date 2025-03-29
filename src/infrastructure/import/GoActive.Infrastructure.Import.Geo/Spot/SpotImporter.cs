using System.Diagnostics.CodeAnalysis;

using GoActive.Infrastructure.Import.Geo.Models;
using GoActive.Modules.Geo.Application.Spot.Create;
using GoActive.Shared.Domain.Enums;
using GoActive.Shared.Serialization.NetTopologySuite;

using Mediator;

using Microsoft.Extensions.Logging;

using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

using static GoActive.Infrastructure.Import.Geo.Spot.SpotConstants;

namespace GoActive.Infrastructure.Import.Geo.Spot;

internal sealed class SpotImporter(ISender sender, ILogger<SpotImporter> logger) : FeaturesImporter(logger)
{
    private readonly HashSet<string> _requiedFields =
    [
        Fields.Title,
        Fields.Activities,
    ];

    internal override async ValueTask<ImportResult> HandleFeatures(IReadOnlyCollection<IFeature> features, CancellationToken cancellation)
    {
        var errorCount = 0;
        var dtos = new List<CreateSpotDto>(features.Count);

        foreach (var feature in features)
        {
            if (!TryMap(feature, out var dto))
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

        var successCount = await sender.Send(new CreateSpotsCommand(dtos), cancellation);

        return new(features.Count, successCount, errorCount);
    }

    private bool TryMap(IFeature feature, [NotNullWhen(true)] out CreateSpotDto? spotDto)
    {
        spotDto = null;

        var missingFields = _requiedFields.Except(feature.Attributes.GetNames()).ToArray();
        if (missingFields.Length != 0)
        {
            logger.LogError("Missing required fields: '{RequiredFields}'", string.Join(", ", missingFields));
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

        var addressIdString = feature.Attributes.GetOptionalValue(Fields.AddressId)?.ToString();
        spotDto = new CreateSpotDto
        {
            Title = feature.Attributes[Fields.Title].ToString()!,
            Activities = activities,
            Latitude = point.X,
            Longitude = point.Y,

            Altitude = point.Z is Coordinate.NullOrdinate ? null : point.Z,
            Description = feature.Attributes.GetOptionalValue(Fields.Description)?.ToString(),
            AddressId = !string.IsNullOrEmpty(addressIdString) && Guid.TryParse(addressIdString, out var addressId) ? addressId : null,
        };
        return true;
    }

    private bool TryExtractActivities(IFeature feature, [NotNullWhen(true)] out IReadOnlyCollection<ActivityTypes>? activities)
    {
        try
        {
            activities = feature.ExtractEnumValues<ActivityTypes>(Fields.Activities);
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
