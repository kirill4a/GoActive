using EFCore.BulkExtensions;

using GoActive.Infrastructure.Storage.Geo.Entities;
using GoActive.Infrastructure.Storage.Geo.Extensions;
using GoActive.Modules.Geo.Application.Shared.Storage;

using Microsoft.EntityFrameworkCore;

namespace GoActive.Infrastructure.Storage.Geo;

internal class GeoContext(DbContextOptions<GeoContext> options) : DbContext(options), IGeoContext, IUnitOfWork
{
    public DbSet<Spot> Spots { get; init; }
    public DbSet<Address> Addresses { get; init; }

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

    async Task IGeoContext.BulkInsertOrUpdateAsync<TEntity>(IEnumerable<TEntity> entities,
                                                            Action<BulkConfig> bulkAction,
                                                            CancellationToken cancellationToken)
    {
        static void DatesAction(BulkConfig cfg)
        {
            cfg.PropertiesToExcludeOnUpdate =
            [
                nameof(Entity.CreatedAt),
                .. cfg.PropertiesToExcludeOnUpdate ?? [],
            ];
        }

        bulkAction += DatesAction;

        await this.BulkInsertOrUpdateAsync(entities,
                                           bulkAction: bulkAction,
                                           type: typeof(TEntity),
                                           cancellationToken: cancellationToken);
    }
}
