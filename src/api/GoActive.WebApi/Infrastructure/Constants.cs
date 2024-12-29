using System.Reflection;

using Asp.Versioning;

namespace GoActive.WebApi.Infrastructure;

internal static class Constants
{
    internal static class Versions
    {
        // WARNING! strange bug
        // if 'api version prefix' contains one symbol only -> this symbol shouldn't be presented in 'api version status'
        // otherwise the route map issue occurs (HTTP 404)
        private const string ApiVesrionStatus = "prealpha";
        internal const string ApiVersionPrefix = "v";
        internal const string ApiVersionFormat = "VVV";

        internal static ApiVersion DefaultApiVersion { get; } = new(1, status: ApiVesrionStatus);
        internal static string ApiName { get; } = Assembly.GetAssembly(typeof(Program))?.GetName()?.Name!;
    }

    internal static class Routes
    {
        internal const string GetSketch = "GetSketch";
        internal const string SearchSpots = "SearchSpots";
    }

    internal static class Cors
    {
        internal const string Localhost = "localhost";
    }
}