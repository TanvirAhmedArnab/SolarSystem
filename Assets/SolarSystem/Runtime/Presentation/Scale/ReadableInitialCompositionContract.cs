namespace Tanvir.SolarSystem.Presentation.Scale
{
    /// <summary>
    /// Owns the reviewed deterministic epoch used for the readable opening composition.
    /// </summary>
    public static class ReadableInitialCompositionContract
    {
        /// <summary>Number of seconds in one mean solar day.</summary>
        public const double SecondsPerDay = 86400d;

        /// <summary>
        /// Shared offset from J2000 that distributes the eight planet directions.
        /// </summary>
        public const double J2000OffsetDays = 4904d;

        /// <summary>Simulation time applied when the explorer scene opens.</summary>
        public const double InitialSimulationTimeSeconds =
            J2000OffsetDays * SecondsPerDay;

        /// <summary>
        /// Minimum reviewed opening angle between any two planet directions.
        /// </summary>
        public const float MinimumPlanetAngularSeparationDegrees = 30f;
    }
}
