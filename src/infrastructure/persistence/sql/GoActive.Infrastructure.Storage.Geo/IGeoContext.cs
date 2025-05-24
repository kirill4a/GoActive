using EFCore.BulkExtensions;

using GoActive.Infrastructure.Storage.Geo.Entities;

using Microsoft.EntityFrameworkCore;

namespace GoActive.Infrastructure.Storage.Geo;

/// <summary>
/// Database context interface.
/// </summary>
internal interface IGeoContext : IDisposable
{
    DbSet<Spot> Spots { get; }

    DbSet<Address> Addresses { get; }

    Task BulkInsertOrUpdateAsync<TEntity>(IEnumerable<TEntity> entities,
                                          Action<BulkConfig> bulkAction,
                                          CancellationToken cancellationToken = default)
        where TEntity : Entity;
}
