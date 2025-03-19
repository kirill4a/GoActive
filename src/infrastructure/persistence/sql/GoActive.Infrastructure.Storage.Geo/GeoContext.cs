using GoActive.Infrastructure.Storage.Geo.Entities;
using GoActive.Infrastructure.Storage.Geo.Extensions;
using GoActive.Modules.Geo.Application.Shared.Storage;

using Microsoft.EntityFrameworkCore;

namespace GoActive.Infrastructure.Storage.Geo;

internal class GeoContext(DbContextOptions<GeoContext> options) : DbContext(options), IGeoContext, IUnitOfWork
{
    public DbSet<Spot> Spots { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .WithPostgresExtensions()
            .ApplyConfigurationsFromAssembly(typeof(GeoContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken)
    {
        SetDates();
        return await SaveChangesAsync(cancellationToken);
    }

    private void SetDates()
    {
        var entries = ChangeTracker.Entries<Entity>().Where(x => x is { State: EntityState.Added or EntityState.Modified });

        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    break;
                default:
                    throw new InvalidOperationException($"Tracked entry state {entry.State} is not supported.");
            }
        }
    }
}
