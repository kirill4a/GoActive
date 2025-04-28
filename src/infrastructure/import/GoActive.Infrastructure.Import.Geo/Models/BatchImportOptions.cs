namespace GoActive.Infrastructure.Import.Geo.Models;

/// <summary>
/// Options for batch import operations.
/// </summary>
public readonly record struct BatchImportOptions : IImportOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BatchImportOptions"/> struct with default properties.
    /// </summary>
    public static BatchImportOptions Default => new();

    /// <summary>
    /// The maximum number of rows to be imported in a single batch.
    /// </summary>
    /// <remarks>
    /// The default value of 0 means all the rows will be imported within a single batch.
    /// </remarks>
    public ushort BatchSize { get; init; }
}
