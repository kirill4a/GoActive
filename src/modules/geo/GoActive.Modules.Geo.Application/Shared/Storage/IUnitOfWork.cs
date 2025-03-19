namespace GoActive.Modules.Geo.Application.Shared.Storage;

/// <summary>
/// Unit of work.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Commit changes to the storage.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<int> CommitAsync(CancellationToken cancellationToken);
}
