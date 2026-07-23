using Tanvir.SolarSystem.Mathematics;

namespace Tanvir.SolarSystem.Simulation
{
    /// <summary>Represents one deterministic evaluated state for a celestial body.</summary>
    public readonly struct CelestialState
    {
        /// <summary>Initializes a celestial state.</summary>
        public CelestialState(
            CelestialBodyId id,
            Double3 parentRelativePositionKm,
            Double3 physicalPositionKm,
            double orbitalSpeedKmPerSec,
            double rotationAngleDeg)
        {
            Id = id;
            ParentRelativePositionKm = parentRelativePositionKm;
            PhysicalPositionKm = physicalPositionKm;
            OrbitalSpeedKmPerSec = orbitalSpeedKmPerSec;
            RotationAngleDeg = rotationAngleDeg;
        }

        /// <summary>Gets the body identifier.</summary>
        public CelestialBodyId Id { get; }
        /// <summary>Gets the parent-relative physical position in kilometers.</summary>
        public Double3 ParentRelativePositionKm { get; }
        /// <summary>Gets the catalog-world physical position in kilometers.</summary>
        public Double3 PhysicalPositionKm { get; }
        /// <summary>Gets instantaneous parent-relative orbital speed in kilometers per second.</summary>
        public double OrbitalSpeedKmPerSec { get; }
        /// <summary>Gets the normalized sidereal rotation angle in degrees.</summary>
        public double RotationAngleDeg { get; }
    }
}
