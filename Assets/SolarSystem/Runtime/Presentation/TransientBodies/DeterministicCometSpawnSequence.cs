using System;
using UnityEngine;

namespace Tanvir.SolarSystem.Presentation.TransientBodies
{
    /// <summary>
    /// Produces a reproducible sequence of bounded comet launch parameters.
    /// </summary>
    public sealed class DeterministicCometSpawnSequence
    {
        private readonly System.Random random;

        /// <summary>Initializes a deterministic sequence from an authored seed.</summary>
        public DeterministicCometSpawnSequence(int seed)
        {
            random = new System.Random(seed);
        }

        /// <summary>Gets the next bounded real-time spawn interval.</summary>
        public float NextInterval(CometSpawnerModel model)
        {
            return NextFloat(
                model.MinimumSpawnIntervalSeconds,
                model.MaximumSpawnIntervalSeconds);
        }

        /// <summary>Creates the next launch through a randomized region around the Sun.</summary>
        public CometSpawnPlan NextPlan(
            CometSpawnerModel model,
            Vector3 spawnPosition,
            Vector3 targetCenter)
        {
            Vector3 target =
                targetCenter + NextPointInsideSphere(model.TargetRadius);
            Vector3 direction = target - spawnPosition;
            if (direction.sqrMagnitude <= 0.000001f)
            {
                direction = Vector3.forward;
            }

            direction.Normalize();
            float speed = NextFloat(model.MinimumSpeed, model.MaximumSpeed);
            float radius = NextFloat(model.MinimumRadius, model.MaximumRadius);
            Vector3 spinAxis = NextUnitVector();
            float spin = NextFloat(
                model.MinimumSpinDegreesPerSecond,
                model.MaximumSpinDegreesPerSecond);
            return new CometSpawnPlan(
                spawnPosition,
                direction * speed,
                radius,
                spinAxis,
                spin);
        }

        private Vector3 NextPointInsideSphere(float radius)
        {
            float volumeRadius =
                Mathf.Pow((float)random.NextDouble(), 1f / 3f) * radius;
            return NextUnitVector() * volumeRadius;
        }

        private Vector3 NextUnitVector()
        {
            float z = NextFloat(-1f, 1f);
            float angle = NextFloat(0f, Mathf.PI * 2f);
            float radial = Mathf.Sqrt(Mathf.Max(0f, 1f - (z * z)));
            return new Vector3(
                radial * Mathf.Cos(angle),
                z,
                radial * Mathf.Sin(angle));
        }

        private float NextFloat(float minimum, float maximum)
        {
            if (Mathf.Approximately(minimum, maximum))
            {
                return minimum;
            }

            return Mathf.Lerp(
                minimum,
                maximum,
                (float)random.NextDouble());
        }
    }
}
