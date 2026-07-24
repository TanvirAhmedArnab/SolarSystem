namespace Tanvir.SolarSystem.Simulation
{
    /// <summary>
    /// Defines the verified Earth quantities used as shared simulation and
    /// presentation references. Authored Earth data is regression-tested against them.
    /// </summary>
    public static class CelestialReferenceUnits
    {
        /// <summary>Verified Earth volumetric mean radius in kilometers.</summary>
        public const double EarthMeanRadiusKm = 6371d;

        /// <summary>Verified Earth sidereal rotation period in seconds.</summary>
        public const double EarthSiderealRotationPeriodSeconds = 86164.2d;
    }
}
