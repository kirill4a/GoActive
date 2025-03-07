namespace GoActive.Infrastructure.Storage.Geo.Entities;

internal abstract class Entity
{
    /// <summary>
    /// Timestamp when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the entity was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
