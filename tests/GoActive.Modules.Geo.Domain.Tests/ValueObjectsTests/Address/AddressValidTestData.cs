namespace GoActive.Modules.Geo.Domain.Tests.ValueObjectsTests;

public class AddressValidTestData : TheoryData<string, string, string, string, string, string>
{
    public AddressValidTestData()
    {
        var country = "Any Country";
        var region = "Any Region";
        var settlement = "Any Settlement";
        var street = "Any Street";
        var building = "Any Building";
        var postCode = "Any Postal Code";

        Add(country, null!, null!, null!, null!, null!);
        Add(country, region, null!, null!, null!, null!);
        Add(country, null!, settlement, null!, null!, null!);
        Add(country, region, settlement, null!, null!, null!);
        Add(country, region, settlement, street, building, null!);
        Add(country, region, settlement, street, null!, postCode);
        Add(country, region, settlement, street, building, postCode);
    }
}
