using System.Reflection;

using GoActive.Infrastructure.Storage.Geo.Repositories;
using GoActive.Modules.Geo.Application.Shared.Storage;
using GoActive.Modules.Geo.Application.Spot.Create;
using GoActive.Modules.Geo.Application.Spot.Search;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoActive.Infrastructure.Storage.Geo.DI;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures geo storage dependencies.
    /// </summary>
    public static IServiceCollection AddGeoStorage(this IServiceCollection services, string connectionString) =>
        services.AddDbContext(connectionString)
                .AddRepositories();

    /// <summary>
    /// Configures geo storage dependencies.
    /// </summary>
    public static IServiceCollection AddGeoStorage(this IServiceCollection services, string connectionString, Assembly migrationsAssembly) =>
        services.AddDbContext(connectionString, migrationsAssembly)
                .AddRepositories();

    /// <summary>
    /// Configures geo storage dependencies.
    /// </summary>
    public static IServiceCollection AddGeoStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Geo");
        return string.IsNullOrWhiteSpace(connectionString)
            ? throw new ArgumentException("Connection string should be specified.")
            : services.AddGeoStorage(connectionString);
    }

    private static IServiceCollection AddDbContext(this IServiceCollection services, string connectionString, Assembly? migrationsAssembly = null)
    {
        services.AddDbContext<GeoContext>(options => options
                                                        .UseNpgsql(connectionString, x =>
                                                        {
                                                            x.UseNetTopologySuite();
                                                            if (migrationsAssembly is not null)
                                                            {
                                                                x.MigrationsAssembly(migrationsAssembly);
                                                            }
                                                        })
                                                        .UseSnakeCaseNamingConvention());

        services.AddScoped<IGeoContext>(sp => sp.GetRequiredService<GeoContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<GeoContext>());

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<SpotRepository>()
                .AddScoped<ISpotCreator>(sp => sp.GetRequiredService<SpotRepository>())
                .AddScoped<ISpotSearcher>(sp => sp.GetRequiredService<SpotRepository>());

        return services;
    }
}
