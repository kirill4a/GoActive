using GoActive.Infrastructure.Storage.Geo.Entities;

using Microsoft.EntityFrameworkCore;

namespace GoActive.Infrastructure.Storage.Geo;

/// <summary>
/// Database context interface.
/// </summary>
internal interface IGeoContext
{
    DbSet<Spot> Spots { get; }
}
