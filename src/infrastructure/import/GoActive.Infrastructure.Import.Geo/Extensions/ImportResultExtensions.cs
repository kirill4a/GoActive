using GoActive.Infrastructure.Import.Geo.Models;

namespace GoActive.Infrastructure.Import.Geo.Extensions;

internal static class ImportResultExtensions
{
    internal static ImportResult Concat(this ImportResult source, ImportResult other) =>
        source with
        {
            Total = source.Total + other.Total,
            Successes = source.Successes + other.Successes,
            Errors = source.Errors + other.Errors,
        };
}
