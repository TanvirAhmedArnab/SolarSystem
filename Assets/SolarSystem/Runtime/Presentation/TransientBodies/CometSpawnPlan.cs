using UnityEngine;

namespace Tanvir.SolarSystem.Presentation.TransientBodies
{
    /// <summary>Describes one immutable deterministic comet launch.</summary>
    public readonly struct CometSpawnPlan
    {
        /// <summary>Initializes one launch plan.</summary>
        public CometSpawnPlan(
            Vector3 position,
            Vector3 velocity,
            float radius,
            Vector3 spinAxis,
            float spinDegreesPerSecond)
        {
            Position = position;
            Velocity = velocity;
            Radius = radius;
            SpinAxis = spinAxis;
            SpinDegreesPerSecond = spinDegreesPerSecond;
        }

        /// <summary>Gets the world-space launch position.</summary>
        public Vector3 Position { get; }

        /// <summary>Gets the world-space velocity.</summary>
        public Vector3 Velocity { get; }

        /// <summary>Gets the illustrative nucleus radius.</summary>
        public float Radius { get; }

        /// <summary>Gets the normalized visual spin axis.</summary>
        public Vector3 SpinAxis { get; }

        /// <summary>Gets the visual spin rate.</summary>
        public float SpinDegreesPerSecond { get; }
    }
}
