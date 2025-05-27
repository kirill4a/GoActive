using System.CommandLine;
using System.CommandLine.Invocation;

using GoActive.Console.Features.Import.Options;
using GoActive.Infrastructure.Import.Geo;
using GoActive.Infrastructure.Import.Geo.Models;

using Microsoft.Extensions.DependencyInjection;

namespace GoActive.Console.Features.Import.Commands;

internal class ImportSpotCommand : Command
{
    private const string CommandName = "spot";

    public ImportSpotCommand()
        : base(CommandName, "Import spot data from a source")
    {
    }

    internal class CommandHandler(IServiceProvider serviceProvider) : ICommandHandler
    {
        public int Invoke(InvocationContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var file = context.ParseResult.GetValueForOption(ImportOptions.FileOption);
            if (!File.Exists(file))
            {
                context.Console.WriteLine($"File not found: {file}");
                return 1;
            }

            await using var scope = serviceProvider.CreateAsyncScope();
            var importer = scope.ServiceProvider.GetRequiredKeyedService<IGeoJsonLinesImporter<BatchImportOptions>>(SpotImportConstants.ImporterKey);

            var batchOptions = GetBatchImportOptions(context);
            using var stream = File.OpenRead(file);
            var result = await importer.Import(stream, batchOptions, context.GetCancellationToken());

            context.Console.WriteLine($"Result: {result}");
            return result.Successes > 0 ? 0 : 1;
        }

        private static BatchImportOptions GetBatchImportOptions(InvocationContext context)
        {
            const ushort defaultBatchSize = 10_000;
            var batchSize = context.ParseResult.GetValueForOption(ImportOptions.BatchSizeOption);

            return new()
            {
                BatchSize = batchSize < 1 ? defaultBatchSize : batchSize,
            };
        }
    }
}
