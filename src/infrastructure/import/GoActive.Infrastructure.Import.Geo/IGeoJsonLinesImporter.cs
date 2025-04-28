using GoActive.Infrastructure.Import.Geo.Models;

namespace GoActive.Infrastructure.Import.Geo;

/// <summary>
/// Interface for importing data (features) from GeoJSON lines stream.
/// </summary>
/// <typeparam name="TOptions">Type of the import options.</typeparam>
public interface IGeoJsonLinesImporter<TOptions>
    where TOptions : IImportOptions
{
    /// <summary>
    /// Imports data (features) from GeoJSON lines stream.
    /// </summary>
    /// <param name="jsonLinesStream">Stream containing GeoJSON rows in <a href="https://jsonlines.org">JSON lines</a> format.</param>
    /// <param name="options">Import options.</param>
    /// <param name="cancellation">Cancellation token.</param>
    Task<ImportResult> Import(Stream jsonLinesStream, TOptions options, CancellationToken cancellation);
}
