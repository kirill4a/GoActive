namespace GoActive.Modules.Geo.Domain.Tests.ValueObjectsTests;

public class AddressWrongTestData : TheoryData<string, string, string, string>
{
    public AddressWrongTestData()
    {
        string[] wrongValues = [null!, string.Empty, " ", " some value", "some value ", " some value "];

        for (var i = 0; i < 4; i++)
        {
            foreach (var wrongValue in wrongValues)
            {
                var item = new string[4];
                item[i] = wrongValue;
                Add(item[0], item[1], item[2], item[3]);
            }
        }

        foreach (var wrongValue in wrongValues)
        {
            Add(wrongValue, wrongValue, wrongValue, wrongValue);
        }
    }
}
