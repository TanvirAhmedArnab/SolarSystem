namespace Tanvir.SolarSystem.Simulation
{
    /// <summary>Stores fixed elliptical orbital elements with units expressed by property names.</summary>
    public readonly struct OrbitalElements
    {
        /// <summary>Initializes a set of elliptical orbital elements.</summary>
        public OrbitalElements(
            double semiMajorAxisKm,
            double eccentricity,
            double inclinationDeg,
            double longitudeAscendingNodeDeg,
            double argumentPeriapsisDeg,
            double meanAnomalyAtEpochDeg,
            double orbitalPeriodSeconds)
        {
            SemiMajorAxisKm = semiMajorAxisKm;
            Eccentricity = eccentricity;
            InclinationDeg = inclinationDeg;
            LongitudeAscendingNodeDeg = longitudeAscendingNodeDeg;
            ArgumentPeriapsisDeg = argumentPeriapsisDeg;
            MeanAnomalyAtEpochDeg = meanAnomalyAtEpochDeg;
            OrbitalPeriodSeconds = orbitalPeriodSeconds;
        }

        /// <summary>Gets the semi-major axis in kilometers.</summary>
        public double SemiMajorAxisKm { get; }
        /// <summary>Gets the dimensionless eccentricity.</summary>
        public double Eccentricity { get; }
        /// <summary>Gets the inclination in degrees.</summary>
        public double InclinationDeg { get; }
        /// <summary>Gets the longitude of the ascending node in degrees.</summary>
        public double LongitudeAscendingNodeDeg { get; }
        /// <summary>Gets the argument of periapsis in degrees.</summary>
        public double ArgumentPeriapsisDeg { get; }
        /// <summary>Gets the mean anomaly at the simulation epoch in degrees.</summary>
        public double MeanAnomalyAtEpochDeg { get; }
        /// <summary>Gets the orbital period in seconds.</summary>
        public double OrbitalPeriodSeconds { get; }
    }
}
