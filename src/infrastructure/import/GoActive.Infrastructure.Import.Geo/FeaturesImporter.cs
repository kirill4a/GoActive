using System.Diagnostics.CodeAnalysis;
using System.Text;

using GoActive.Infrastructure.Import.Geo.Extensions;
using GoActive.Infrastructure.Import.Geo.Models;
using GoActive.Shared.Serialization.NetTopologySuite;

using Microsoft.Extensions.Logging;

using NetTopologySuite.Features;

namespace GoActive.Infrastructure.Import.Geo;

internal abstract class FeaturesImporter<TOptions>(ILogger logger) : IGeoJsonLinesImporter<TOptions>
    where TOptions : IImportOptions
{
    protected abstract HashSet<string>? RequiedFields { get; }
    protected abstract BatchImportOptions? GetBatchOptions(TOptions options);
    protected abstract ValueTask<ImportResult> HandleFeatures(IReadOnlyCollection<IFeature> features,
                                                              TOptions options,
                                                              CancellationToken cancellation);

    protected bool ValidateRequiredFields(IFeature feature)
    {
        if (RequiedFields is null)
        {
            return true;
        }

        var missingFields = RequiedFields.Except(feature.Attributes.GetNames()).ToArray();
        if (missingFields.Length != 0)
        {
            logger.LogError("Missing required fields: '{RequiredFields}'", string.Join(", ", missingFields));
            return false;
        }

        return true;
    }

    protected bool ValidateRequiredValue(IFeature feature, string fieldName, [NotNullWhen(true)] out string? value)
    {
        value = feature.Attributes[fieldName].ToString();
        if (string.IsNullOrWhiteSpace(value))
        {
            logger.LogError("Attribute '{FieldName}' shouldn't be empty. {Feature}", fieldName, feature);
            return false;
        }

        return true;
    }

    public async Task<ImportResult> Import(Stream jsonLinesStream, TOptions options, CancellationToken cancellation)
    {
        var deserializer = new GeoJsonSerializer();
        using var reader = new StreamReader(jsonLinesStream, Encoding.UTF8);

        var batchOptions = GetBatchOptions(options) ?? BatchImportOptions.Default;
        var batch = new List<IFeature>(batchOptions.BatchSize);
        var error = 0;

        try
        {
            var result = new ImportResult();
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

                    batch.Add(feature);
                    if (batch.Count == batchOptions.BatchSize)
                    {
                        result = await ImportBatch(batch, options, result, cancellation);

                        // TODO: emit batch imported event
                    }
                }
                catch (Exception ex)
                {
                    error++;
                    logger.LogError(ex, "Failed to deserialize a feature from json line: '{Line}'", line);
                }
            }

            result = await ImportBatch(batch, options, result, cancellation);
            return result with { Errors = result.Errors + error };
        }
        finally
        {
            reader.Close();
        }
    }

    private async ValueTask<ImportResult> ImportBatch(List<IFeature> batch,
                                                      TOptions options,
                                                      ImportResult totalResult,
                                                      CancellationToken cancellation)
    {
        var batchResult = await HandleFeatures(batch, options, cancellation);
        batch.Clear();

        return totalResult.Concat(batchResult);
    }
}
