namespace GoActive.WebApi.Endpoints.NordicSki.Models;

// TODO: convert 'SkiingStyle' to enum
public record SkiTrailPathDto(string SkiingStyle,
                              double Length,
                              double MinHeight,
                              double MaxHeight,
                              double AscendingSummary,
                              double DescendingSummary);
