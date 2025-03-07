using System.Text.Json;
using System.Text.Json.Serialization;

namespace GoActive.Shared.Infrastructure;

/// <summary>
/// Parameters of JSON serializer for default.
/// </summary>
public static class JsonSerializerCustom
{
    /// <summary>
    /// Options for enums serialization.
    /// </summary>
    public static readonly JsonSerializerOptions EnumSerializerOptions =
        new()
        {
            Converters = { new JsonStringEnumConverter() },
        };

    /// <summary>
    /// Options for properties serialization.
    /// </summary>
    public static readonly JsonSerializerOptions PropertySerializerOptions =
        new()
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
}
