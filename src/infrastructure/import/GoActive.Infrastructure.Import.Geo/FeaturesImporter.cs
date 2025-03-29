using System.Text;

using GoActive.Infrastructure.Import.Geo.Models;
using GoActive.Shared.Serialization.NetTopologySuite;

using Microsoft.Extensions.Logging;

using NetTopologySuite.Features;

namespace GoActive.Infrastructure.Import.Geo;

internal abstract class FeaturesImporter(ILogger logger) : IGeoJsonLinesImporter
{
    internal abstract ValueTask<ImportResult> HandleFeatures(IReadOnlyCollection<IFeature> features, CancellationToken cancellation);

    public async Task<ImportResult> Import(Stream jsonLinesStream, CancellationToken cancellation)
    {
        var deserializer = new GeoJsonSerializer();
        using var reader = new StreamReader(jsonLinesStream, Encoding.UTF8);

        var error = 0;
        var features = new List<IFeature>();

        try
        {
            string? line;
            while ((line = await reader.ReadLineAsync(cancellation)) is not null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                try
                {
                    var feature = deserializer.Deserialize<IFeature>(line)
                        ?? throw new InvalidOperationException($"Feature is null. The source line is: '{line}'");

                    features.Add(feature);
                }
                catch (Exception ex)
                {
                    error++;
                    logger.LogError(ex, "Failed to deserialize a feature from json line: '{Line}'", line);
                }
            }

            var result = await HandleFeatures(features, cancellation);
            return result with { Errors = result.Errors + error };
        }
        finally
        {
            reader.Close();
        }
    }
}
