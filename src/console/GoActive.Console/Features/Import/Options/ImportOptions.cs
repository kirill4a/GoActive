using System.CommandLine;

namespace GoActive.Console.Features.Import.Options;

internal static class ImportOptions
{
    internal static readonly Option<string> FileOption = new(aliases: ["-f", "--file"], description: "Path to the file")
    {
        ArgumentHelpName = "file",
        IsRequired = true,
    };

    internal static readonly Option<ushort> BatchSizeOption = new(aliases: ["-b", "--batch"],
                                                                  description: "Size of the batch for import in batch manner")
    {
        ArgumentHelpName = "batchSize",
        IsRequired = false,
    };

    internal static class OpenAddresses
    {
        internal static readonly Option<string> CountryCodeOption = new(aliases: ["-c", "--country"],
                                                                        description: "Country code for OpenAddresses import")
        {
            ArgumentHelpName = "countryCode",
            IsRequired = false,
        };
    }
}
