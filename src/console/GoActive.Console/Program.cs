using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;

using GoActive.Console;
using GoActive.Infrastructure.Storage.Geo.DI;

using Microsoft.Extensions.Hosting;

using Microsoft.Extensions.DependencyInjection;

var rootCommand = new GoActive.Console.RootCommand();

if (args.Length == 0)
{
    await rootCommand.InvokeAsync("-h");
    return 0;
}

var parser = new CommandLineBuilder(rootCommand)
    .UseDefaults()
    .UseHost(
        _ => Host.CreateDefaultBuilder(args),
        host =>
                {
                    host.ConfigureServices(services =>
                    {
                        services.AddGeoStorage()
                                .AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped)
                                .AddImports();
                    })
                    .UseCommandHandlers();
                })
    .Build();

var exitCode = await ExecuteWithMeasure(() => parser.InvokeAsync(args));
return exitCode;

static async Task<int> ExecuteWithMeasure(Func<Task<int>> action)
{
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
    var result = await action();
    stopwatch.Stop();

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"Execution time: {stopwatch.Elapsed}");
    Console.ResetColor();

    return result;
}