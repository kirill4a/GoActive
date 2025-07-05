using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GoActive.Infrastructure.Storage.Geo.Converters;

/// <summary>
/// Converter for enum arrays stored as strings in the database.
/// This converter serializes the enum values to a comma-separated string for storage,
/// and deserializes the string back to an enum array when reading from the database.
/// </summary>
/// <typeparam name="T">Type of the stored enum</typeparam>
internal class EnumArrayConverter<T>() : ValueConverter<T, string>(e => e.ToString(), s => Enum.Parse<T>(s, true))
    where T : struct, Enum;