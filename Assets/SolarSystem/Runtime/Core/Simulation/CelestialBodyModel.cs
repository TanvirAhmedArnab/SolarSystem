namespace Tanvir.SolarSystem.Simulation
{
    /// <summary>Represents an immutable, unit-explicit celestial body definition used at runtime.</summary>
    public sealed class CelestialBodyModel
    {
        /// <summary>Initializes an immutable celestial body model.</summary>
        public CelestialBodyModel(
            CelestialBodyId id,
            string displayName,
            CelestialBodyCategory category,
            CelestialBodyId? parentId,
            double meanRadiusKm,
            double? massKg,
            double rotationPeriodSeconds,
            double axialTiltDeg,
            OrbitalElements? orbit,
            string scientificSourceId)
        {
            Id = id;
            DisplayName = displayName;
            Category = category;
            ParentId = parentId;
            MeanRadiusKm = meanRadiusKm;
            MassKg = massKg;
            RotationPeriodSeconds = rotationPeriodSeconds;
            AxialTiltDeg = axialTiltDeg;
            Orbit = orbit;
            ScientificSourceId = scientificSourceId;
        }

        /// <summary>Gets the stable body identifier.</summary>
        public CelestialBodyId Id { get; }
        /// <summary>Gets the human-readable display name.</summary>
        public string DisplayName { get; }
        /// <summary>Gets the body category.</summary>
        public CelestialBodyCategory Category { get; }
        /// <summary>Gets the parent ID, or null for the catalog root.</summary>
        public CelestialBodyId? ParentId { get; }
        /// <summary>Gets the mean radius in kilometers.</summary>
        public double MeanRadiusKm { get; }
        /// <summary>Gets the optional mass in kilograms.</summary>
        public double? MassKg { get; }
        /// <summary>Gets the signed sidereal rotation period in seconds.</summary>
        public double RotationPeriodSeconds { get; }
        /// <summary>Gets the axial tilt in degrees.</summary>
        public double AxialTiltDeg { get; }
        /// <summary>Gets the orbit relative to the parent, or null for the root.</summary>
        public OrbitalElements? Orbit { get; }
        /// <summary>Gets the stable scientific provenance record ID.</summary>
        public string ScientificSourceId { get; }
    }
}
