using System.CommandLine.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using GoActive.Infrastructure.Storage.Geo.DI;
using GoActive.Console.Features.Import;
using GoActive.Infrastructure.Import.Geo.Extensions;

using GoActive.Console.Features.Import.Commands;
using GoActive.Console.Features.Export.Commands;

namespace GoActive.Console;

internal static class Extensions
{
    internal static IServiceCollection AddGeoStorage(this IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddUserSecrets<Program>()
                                    .AddEnvironmentVariables()
                                    .Build();

        const string connectionStringName = "Geo";
        var connectionString = configuration.GetConnectionString(connectionStringName)
            ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' not found.");

        var migrationsAssembly = typeof(GoActive.Infrastructure.Storage.Geo.Migrations.Program).Assembly;
        services.AddGeoStorage(connectionString, migrationsAssembly);

        return services;
    }

    internal static IServiceCollection AddImports(this IServiceCollection services)
        =>
        services.AddSpotImport(SpotImportConstants.ImporterKey)
                .AddOpenAdressesImport()
                .AddScoped<AddressImportStrategy>();

    internal static IHostBuilder UseCommandHandlers(this IHostBuilder hostBuilder)
        =>
        hostBuilder.UseCommandHandler<ImportAddressCommand, ImportAddressCommand.CommandHandler>()
                   .UseCommandHandler<ImportSpotCommand, ImportSpotCommand.CommandHandler>()
                   .UseCommandHandler<ExportSpotCommand, ExportSpotCommand.CommandHandler>();
}
