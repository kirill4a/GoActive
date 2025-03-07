using System.Text.Json;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoActive.Infrastructure.Storage.Geo.Extensions;

internal static class PropertyBuilderExtensions
{
    private static ValueComparer<IReadOnlyCollection<T>> GetReadOnlyCollectionComparer<T>()
        where T : notnull
        =>
        new(
            (x, y) => (x == null && y == null) || (x != null && y != null && x.SequenceEqual(y)),
            x => x.Aggregate(0, (hash, obj) => HashCode.Combine(hash, obj.GetHashCode())),
            x => x);

    internal static PropertyBuilder<IReadOnlyCollection<T>> HasReadOnlyCollectionJsonConversion<T>(
        this PropertyBuilder<IReadOnlyCollection<T>> builder,
        JsonSerializerOptions? options = null)
        where T : notnull
    {
        return builder.HasConversion(
            x => JsonSerializer.Serialize(x, options),
            x => JsonSerializer.Deserialize<IReadOnlyCollection<T>>(x, options) ?? Array.Empty<T>(),
            GetReadOnlyCollectionComparer<T>());
    }

    internal static PropertyBuilder<T> HasEnumConversion<T>(
        this PropertyBuilder<T> builder,
        JsonSerializerOptions? options = null)
        where T : struct, Enum
    {
        return builder.HasConversion(
            x => x.ToString(),
            x => Enum.Parse<T>(x, true));
    }
}
