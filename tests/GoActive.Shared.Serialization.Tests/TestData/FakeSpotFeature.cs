namespace GoActive.Shared.Serialization.Tests.TestData;

public sealed class FakeSpotFeature : FakeFeature
{
    internal static readonly Domain.Enums.ActivityType[] Activities = [Domain.Enums.ActivityType.NordicSki, Domain.Enums.ActivityType.Workout];

    public FakeSpotFeature()
    {
        WithAttribute("activities", Activities);
    }
}
