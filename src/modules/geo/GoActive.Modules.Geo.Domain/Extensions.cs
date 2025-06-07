using GoActive.Modules.Geo.Domain.ValueObjects;

namespace GoActive.Modules.Geo.Domain;

internal static class Extensions
{
    internal static NormalizedTitle ToNormalized(this Title title)
    {
        ArgumentNullException.ThrowIfNull(title);

        return NormalizedTitle.FromValue(title.Value);
    }
}
