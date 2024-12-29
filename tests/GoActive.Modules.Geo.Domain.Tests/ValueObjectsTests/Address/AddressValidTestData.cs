namespace GoActive.Modules.Geo.Domain.Tests.ValueObjectsTests;

public class AddressValidTestData : TheoryData<string, string, string, string>
{
    public AddressValidTestData()
    {
        var country = "Any Country";
        var region = "Any Region";
        var settlement = "Any Settlement";
        var street = "Any Street";

        Add(country, null!, null!, null!);
        Add(country, region, null!, null!);
        Add(country, null!, settlement, null!);
        Add(country, region, settlement, null!);
        Add(country, region, settlement, street);
    }
}
