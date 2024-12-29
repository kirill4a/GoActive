using System.Text.Json.Serialization;

namespace GoActive.Shared.Domain.Enums;

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
    Workout = 2,

    /// <summary>
    /// The roller (summer) skiing
    /// </summary>
    RollerSki = 3,

    /// <summary>
    /// The biathlon
    /// </summary>
    Biathlon = 4,
}
