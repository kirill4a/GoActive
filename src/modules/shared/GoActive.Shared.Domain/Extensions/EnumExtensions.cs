namespace GoActive.Shared.Domain.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Checks if flagged enum value is defined in Enum
    /// </summary>
    /// <param name="flag">Value of Enum<T></param>
    /// <returns>True if value is in Enum<T>, otherwise false</returns>
    /// <typeparam name="T"/>Type of enum.</typeparam>
    public static bool IsFlagSuitable<T>(this T flag)
        where T : struct, Enum
    {
        var flagValue = Convert.ToInt32(flag);

        if (!Enum.IsDefined(typeof(T), 0) && flagValue == 0)
            return false;

        var entities = Enum.GetValues(typeof(T));
        var composite = 0;
        foreach (var entity in entities)
            composite |= (int)entity;

        return (composite | flagValue) == composite;
    }
}