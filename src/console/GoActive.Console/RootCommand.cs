using GoActive.Console.Features.Export.Commands;
using GoActive.Console.Features.Import.Commands;

namespace GoActive.Console;

public class RootCommand : System.CommandLine.RootCommand
{
    public RootCommand()
        : base("GoActive CLI")
    {
        AddCommands();
    }

    private void AddCommands()
    {
        AddCommand(new ImportCommand());
        AddCommand(new ExportCommand());
    }
}
