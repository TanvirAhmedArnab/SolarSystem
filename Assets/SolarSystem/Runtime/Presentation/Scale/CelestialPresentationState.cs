using Tanvir.SolarSystem.Simulation;
using UnityEngine;

namespace Tanvir.SolarSystem.Presentation.Scale
{
    /// <summary>Represents one projected, Unity-ready celestial state.</summary>
    public readonly struct CelestialPresentationState
    {
        /// <summary>Initializes a projected celestial state.</summary>
        public CelestialPresentationState(
            CelestialBodyId id,
            Vector3 position,
            float displayRadius,
            float rotationAngleDeg)
        {
            Id = id;
            Position = position;
            DisplayRadius = displayRadius;
            RotationAngleDeg = rotationAngleDeg;
        }

        /// <summary>Gets the stable body ID.</summary>
        public CelestialBodyId Id { get; }

        /// <summary>Gets the projected Unity-world position.</summary>
        public Vector3 Position { get; }

        /// <summary>Gets the radius in the active, explicitly disclosed scale.</summary>
        public float DisplayRadius { get; }

        /// <summary>Gets the deterministic sidereal rotation angle.</summary>
        public float RotationAngleDeg { get; }
    }
}
