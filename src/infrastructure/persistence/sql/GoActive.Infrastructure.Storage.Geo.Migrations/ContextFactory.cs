using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GoActive.Infrastructure.Storage.Geo.Migrations;

internal sealed class ContextFactory : IDesignTimeDbContextFactory<GeoContext>
{
    public GeoContext CreateDbContext(string[] args)
    {

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("Geo");

        var optionsBuilder = new DbContextOptionsBuilder<GeoContext>()
        .UseNpgsql(connectionString, x =>
        {
            x.UseNetTopologySuite();
            x.MigrationsAssembly(typeof(Program).Assembly);
        })
        .UseSnakeCaseNamingConvention();

        return new(optionsBuilder.Options);
    }
}
