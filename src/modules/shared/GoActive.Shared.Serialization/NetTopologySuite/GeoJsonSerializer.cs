using System.Text.Json;
using System.Text.Json.Serialization;

using NetTopologySuite.Features;

using NetTopologySuite.IO.Converters;

namespace GoActive.Shared.Serialization.NetTopologySuite;

/// <summary>
/// Serializer for <see cref="NetTopologySuite.Features"/> and <see cref="NetTopologySuite.Geometries"> types.
/// </summary>
public sealed class GeoJsonSerializer
{
    private readonly JsonConverterFactory _enumConverter = new JsonStringEnumConverter();
    private readonly JsonConverterFactory _geoJsonConverter = new GeoJsonConverterFactory();

    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public GeoJsonSerializer(bool writeIndented = true)
    {
        _options.WriteIndented = writeIndented;
        _options.Converters.Add(_enumConverter);
        _options.Converters.Add(_geoJsonConverter);
    }

    public GeoJsonSerializer(JsonSerializerOptions options)
    {
        _options = options;
        _options.Converters.Add(_enumConverter);
        _options.Converters.Add(_geoJsonConverter);
    }

    /// <summary>
    /// Serializes <see cref="IFeature"> to GeoJSON string.
    /// </summary>
    /// <param name="feature">Feature object.</param>
    public string SerializeFeature(IFeature feature)
    {
        return JsonSerializer.Serialize(feature, _options);
    }

    /// <summary>
    /// Deserializes JSON string to specified type.
    /// </summary>
    /// <typeparam name="T">Type of object to deserialize. Should be of <see cref="NetTopologySuite.Features"/> or <see cref="NetTopologySuite.Geometries"> types.</typeparam>
    /// <remarks>
    /// <exception cref="ArgumentException">Thrown when <paramref name="json"/> is null, empty string or whitespace.</exception>
    /// </remarks>
    public T? Deserialize<T>(string json)
        where T : class
    {
        if (!_geoJsonConverter.CanConvert(typeof(T)))
        {
            throw new ArgumentException($"Type '{typeof(T)}' is not supported by {nameof(GeoJsonSerializer)}.");
        }

        return string.IsNullOrWhiteSpace(json)
            ? throw new ArgumentException("Value cannot be null, empty string or whitespace.", nameof(json))
            : JsonSerializer.Deserialize<T>(json, _options);
    }
}
