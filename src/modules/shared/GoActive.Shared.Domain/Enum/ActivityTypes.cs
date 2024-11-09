using System.Text.Json.Serialization;

namespace GoActive.Shared.Domain.Enum;

/// <summary>
/// Available types of activities
/// </summary>
[Flags]
[JsonConverter(typeof(JsonStringEnumConverter<ActivityTypes>))]
public enum ActivityTypes
{
    /// <summary>
    /// The nordic skiing
    /// </summary>
    NordicSki = 1,

    /// <summary>
    /// Workout aka Calisthenics
    /// </summary>
    Workout = 2
}
