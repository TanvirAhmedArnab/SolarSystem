namespace Tanvir.SolarSystem.Presentation.Scale
{
    /// <summary>
    /// Owns the reviewed numeric contract for the compressed, readable overview.
    /// </summary>
    public static class ReadableOverviewScaleContract
    {
        /// <summary>Physical kilometers represented by the first log interval.</summary>
        public const double DistanceReferenceKm = 1000000d;

        /// <summary>Display units allocated to each base-10 distance interval.</summary>
        public const double UnitsPerDistanceDecade = 160d;

        /// <summary>Earth's display radius in the proportional body-size scale.</summary>
        public const double EarthDisplayRadius = 1d;

        /// <summary>Conservative surface gap required between adjacent orbit envelopes.</summary>
        public const double MinimumSurfaceClearance = 2.5d;

        /// <summary>
        /// Minimum invisible selection radius for sub-pixel proportional bodies.
        /// </summary>
        public const float MinimumSelectionRadius = 1.5f;

        /// <summary>
        /// Compensates cached orbit-path width for the expanded overview envelope.
        /// </summary>
        public const float OrbitPathWidthMultiplier = 10f;

        /// <summary>
        /// Saturn's generated ring edge measured in Saturn body radii.
        /// </summary>
        public const double SaturnRingOuterRadiusMultiplier = 2.3d;
    }
}
