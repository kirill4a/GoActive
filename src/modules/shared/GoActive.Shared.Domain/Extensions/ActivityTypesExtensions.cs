using GoActive.Shared.Domain.Enums;

namespace GoActive.Shared.Domain.Extensions;

public static class ActivityTypesExtensions
{
    private static readonly ActivityTypes[] AllActivities = Enum.GetValues<ActivityTypes>();

    public static ActivityTypes[] FlagsToArray(this ActivityTypes flag) =>
        AllActivities.Where(x => flag.HasFlag(x)).ToArray();
}
