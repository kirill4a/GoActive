namespace GoActive.Modules.Geo.Domain;

public static class Constants
{
    /// <summary>
    /// Constants for Spatial Reference Identifiers (SRID).
    /// These are used to define the coordinate system for geographic data.
    /// </summary>
    public static class Srid
    {
        /// <summary>
        /// World Geodetic System 1984
        /// </summary>
        public const int Wgs84 = 4326;

        /// <summary>
        /// Pulkovo 1942
        public const int Pulkovo1942 = 4284;
    }
}
