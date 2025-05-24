using System.CommandLine;

using GoActive.Console.Features.Import.Options;

namespace GoActive.Console.Features.Import.Commands;

public class ImportCommand : Command
{
    private const string CommandName = "import";

    public ImportCommand()
        : base(CommandName, "Import data from a source")
    {
        AddCommand(new ImportAddressCommand());
        AddCommand(new ImportSpotCommand());
        AddGlobalOption(ImportOptions.FileOption);
    }
}
