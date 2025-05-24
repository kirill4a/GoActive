using System.CommandLine;
using System.CommandLine.Invocation;

using GoActive.Console.Features.Import.Options;

namespace GoActive.Console.Features.Import.Commands;

internal class ImportAddressCommand : Command
{
    private const string CommandName = "address";
    private static readonly AddressSourceOption _sourceOption = new();

    public ImportAddressCommand()
        : base(CommandName, "Import address data from a source")
    {
        AddOption(_sourceOption);
        AddOption(ImportOptions.BatchSizeOption);
        AddOption(ImportOptions.OpenAddresses.CountryCodeOption);
    }

    internal class CommandHandler(AddressImportStrategy importStrategy) : ICommandHandler
    {
        public int Invoke(InvocationContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var source = context.ParseResult.GetValueForOption(_sourceOption);
            var file = context.ParseResult.GetValueForOption(ImportOptions.FileOption);

            if (!File.Exists(file))
            {
                context.Console.WriteLine($"File not found: {file}");
                return 1;
            }

            var importAction = importStrategy.GetImportAction(source, context);

            using var stream = File.OpenRead(file);
            var result = await importAction(stream, context.GetCancellationToken());

            context.Console.WriteLine($"Result: {result}");
            return result.Successes > 0 ? 0 : 1;
        }
    }
}
