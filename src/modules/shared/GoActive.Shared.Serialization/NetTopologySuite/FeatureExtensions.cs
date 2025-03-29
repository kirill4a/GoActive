using NetTopologySuite.Features;

namespace GoActive.Shared.Serialization.NetTopologySuite;

public static class FeatureExtensions
{
    /// <summary>
    /// Tries to extract the enum values of the specified attribute from the feature.
    /// </summary>
    /// <typeparam name="TEnum">Enum type.</typeparam>
    /// <param name="feature">Instance of <see cref="IFeature"/>.</param>
    /// <param name="attributeName">The name of attribute which contains value in form of enum array.</param>
    /// <returns>The collection of <see cref="TEnum"> values if attribute with given name exists and contains value of enums. Othewise null.</returns>
    /// <exception cref="ArgumentException">Throws if feature is null or attribute name is null, empty or whitespace string.</exception>
    public static IReadOnlyCollection<TEnum>? ExtractEnumValues<TEnum>(this IFeature feature, string attributeName)
        where TEnum : struct, Enum
    {
        ArgumentNullException.ThrowIfNull(feature);
        if (string.IsNullOrWhiteSpace(attributeName))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(attributeName));
        }

        var arrayValue = feature.Attributes.GetOptionalValue(attributeName);

        if (arrayValue is object[] objectArray)
        {
            return [.. ParseEnumNames<TEnum>(objectArray)];
        }

        if (arrayValue is not TEnum[] enumArray || typeof(TEnum) != enumArray.GetType().GetElementType())
        {
            return null;
        }

        return enumArray;
    }

    private static IEnumerable<TEnum> ParseEnumNames<TEnum>(object[] enumNames)
        where TEnum : struct, Enum
    {
        foreach (var item in enumNames)
        {
            var name = item?.ToString();
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            if (!Enum.TryParse<TEnum>(name, ignoreCase: true, out var enumValue)
                || !Enum.IsDefined(enumValue))
            {
                throw new InvalidCastException($"Cannot convert '{name}' value to {typeof(TEnum).Name} enum.");
            }

            yield return enumValue;
        }
    }
}
