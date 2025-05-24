using System.CommandLine;

using GoActive.Modules.Geo.Application.Address;

namespace GoActive.Console.Features.Import.Options;

internal class AddressSourceOption : Option<AddressSource>
{
    public AddressSourceOption()
        : base(name: "--source", description: "Address source (OpenAddresses, etc)")
    {
        IsRequired = true;
        AddAlias("-s");
    }
}
