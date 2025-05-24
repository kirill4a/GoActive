using System.CommandLine;
using System.CommandLine.Invocation;

using GoActive.Console.Features.Import.Options;

namespace GoActive.Console.Features.Import.Commands;

internal class ImportSpotCommand : Command
{
    private const string CommandName = "spot";

    public ImportSpotCommand()
        : base(CommandName, "Import spot data from a source")
    {
        this.SetHandler(async (InvocationContext context) =>
                {
                    var file = context.ParseResult.GetValueForOption(ImportOptions.FileOption);
                    if (!File.Exists(file))
                    {
                        context.Console.WriteLine($"File not found: {file}");
                        return;
                    }

                    await Handle(file!);
                });
    }

    private static Task Handle(string file)
    {
        throw new NotImplementedException("Spot import functionality is not implemented yet.");
    }
}
