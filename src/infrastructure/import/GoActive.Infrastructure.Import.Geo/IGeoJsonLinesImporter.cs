using GoActive.Infrastructure.Import.Geo.Models;

namespace GoActive.Infrastructure.Import.Geo;

public interface IGeoJsonLinesImporter
{
    /// <summary>
    /// Imports data (features) from GeoJSON lines stream.
    /// </summary>
    /// <param name="jsonLinesStream">Stream containing GeoJSON rows in <a href="https://jsonlines.org">JSON lines</a> format.</param>
    /// <param name="cancellation">Cancellation token.</param>
    Task<ImportResult> Import(Stream jsonLinesStream, CancellationToken cancellation);
}
