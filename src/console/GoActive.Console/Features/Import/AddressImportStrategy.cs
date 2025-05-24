using System.CommandLine.Invocation;

using GoActive.Console.Features.Import.Options;

using GoActive.Infrastructure.Import.Geo;
using GoActive.Infrastructure.Import.Geo.Address.OpenAddresses;
using GoActive.Infrastructure.Import.Geo.Models;
using GoActive.Modules.Geo.Application.Address;

using Microsoft.Extensions.DependencyInjection;

namespace GoActive.Console.Features.Import;

internal class AddressImportStrategy(IServiceProvider serviceProvider)
{
    internal Func<Stream, CancellationToken, Task<ImportResult>> GetImportAction(AddressSource addressSource, InvocationContext context)
    {
        Func<Stream, CancellationToken, Task<ImportResult>> func = addressSource switch
        {
            AddressSource.OpenAddresses => (stream, cancellationToken) =>
                serviceProvider
                    .GetRequiredService<IGeoJsonLinesImporter<OpenAddressesImportOptions>>()
                    .Import(stream, GetOpenAddressesOptions(context), cancellationToken),

            _ => throw new NotSupportedException($"Import from address source '{addressSource}' is not supported"),
        };

        return func;
    }

    private static OpenAddressesImportOptions GetOpenAddressesOptions(InvocationContext context)
    {
        const ushort defaultBatchSize = 10_000;
        var batchSize = context.ParseResult.GetValueForOption(ImportOptions.BatchSizeOption);

        var countryCode = context.ParseResult.GetValueForOption(ImportOptions.OpenAddresses.CountryCodeOption)?.Trim();
        while (string.IsNullOrWhiteSpace(countryCode) || countryCode.Length != 2)
        {
            context.Console.Out.Write("Country code of 2 letters is required for OpenAddresses import. Please enter country code (2-aplha): ");
            countryCode = System.Console.In.ReadLine()?.Trim();
        }

        return new()
        {
            BatchOptions = new() { BatchSize = batchSize < 1 ? defaultBatchSize : batchSize },
            CountryCode = countryCode.ToUpperInvariant(),
        };
    }
}
