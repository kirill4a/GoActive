using System.CommandLine;

using GoActive.Console.Features.Export.Options;

namespace GoActive.Console.Features.Export.Commands;

internal class ExportCommand : Command
{
    private const string CommandName = "export";

    public ExportCommand()
        : base(CommandName, "Export data")
    {
        AddCommand(new ExportSpotCommand());
        AddGlobalOption(ExportOptions.FileOption);
    }
}
