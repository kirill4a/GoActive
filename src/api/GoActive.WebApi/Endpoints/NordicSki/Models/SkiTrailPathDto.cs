namespace GoActive.WebApi.Endpoints.NordicSki.Models;

public record SkiTrailPathDto(string SkiingStyle,//TODO: convert to enum
                              double Length,                                    
                              double MinHeight,
                              double MaxHeight,
                              double AscendingSummary,
                              double DescendingSummary);
