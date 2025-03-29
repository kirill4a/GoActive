namespace GoActive.Shared.Serialization.Tests.TestData;

public sealed class FakeSpotFeature : FakeFeature
{
    internal static readonly Domain.Enums.ActivityTypes[] Activities = [Domain.Enums.ActivityTypes.NordicSki, Domain.Enums.ActivityTypes.Workout];

    public FakeSpotFeature()
    {
        WithAttribute("activities", Activities);
    }
}
