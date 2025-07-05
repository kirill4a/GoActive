using System.Text.Json;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoActive.Infrastructure.Storage.Geo.Extensions;

internal static class PropertyBuilderExtensions
{
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
