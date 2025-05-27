using System.CommandLine;
using System.CommandLine.Invocation;

using GoActive.Console.Features.Export.Options;

using Microsoft.Extensions.DependencyInjection;

namespace GoActive.Console.Features.Export.Commands;

internal class ExportSpotCommand : Command
{
    private const string CommandName = "spot";

    public ExportSpotCommand()
        : base(CommandName, "Export spot data")
    {
    }

    internal class CommandHandler() : ICommandHandler
    {
        public int Invoke(InvocationContext context)
        {
            throw new NotImplementedException();
        }

        public Task<int> InvokeAsync(InvocationContext context)
        {
            throw new NotImplementedException();
        }
    }
}