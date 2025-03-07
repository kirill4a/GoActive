using Microsoft.EntityFrameworkCore;

namespace GoActive.Infrastructure.Storage.Geo.Extensions;

internal static class PostgresExtensions
{
    internal static ModelBuilder WithPostgresExtensions(this ModelBuilder modelBuilder) =>
        modelBuilder.HasPostgresExtension("btree_gin")
                    .HasPostgresExtension("pg_trgm");
}
