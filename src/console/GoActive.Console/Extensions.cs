using GoActive.Infrastructure.Storage.Geo.DI;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
}
