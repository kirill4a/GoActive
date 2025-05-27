using System.CommandLine;

namespace GoActive.Console.Features.Export.Options;

internal static class ExportOptions
{
    internal static readonly Option<string> FileOption = new(aliases: ["-f", "--file"], description: "The file path to save export results")
    {
        ArgumentHelpName = "file",
        IsRequired = true,
    };
}
