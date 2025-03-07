using GoActive.Infrastructure.Storage.Geo.Entities;
using GoActive.Infrastructure.Storage.Geo.Extensions;

using Microsoft.EntityFrameworkCore;

namespace GoActive.Infrastructure.Storage.Geo;

internal class GeoContext(DbContextOptions<GeoContext> options) : DbContext(options), IGeoContext
{
    public DbSet<Spot> Spots { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .WithPostgresExtensions()
            .ApplyConfigurationsFromAssembly(typeof(GeoContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
