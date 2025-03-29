namespace GoActive.Infrastructure.Import.Geo.Models;

/// <summary>
/// The result of import operation.
/// </summary>
/// <param name="Total">Total count of handled non-empty rows.</param>
/// <param name="Successes">Count of successfully imported rows.</param>
/// <param name="Errors">Count of not imported rows.</param>
public readonly record struct ImportResult(int Total, int Successes, int Errors);