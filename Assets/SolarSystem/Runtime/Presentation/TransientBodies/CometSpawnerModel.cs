using System;

namespace Tanvir.SolarSystem.Presentation.TransientBodies
{
    /// <summary>
    /// Defines immutable presentation and pooling limits for transient comets.
    /// </summary>
    public readonly struct CometSpawnerModel
    {
        /// <summary>Initializes validated comet-spawner parameters.</summary>
        public CometSpawnerModel(
            int randomSeed,
            int poolSize,
            float initialSpawnDelaySeconds,
            float minimumSpawnIntervalSeconds,
            float maximumSpawnIntervalSeconds,
            float orbitRadius,
            float orbitPeriodSimulationDays,
            float orbitInclinationDegrees,
            float targetRadius,
            float minimumSpeed,
            float maximumSpeed,
            float minimumRadius,
            float maximumRadius,
            float minimumSpinDegreesPerSecond,
            float maximumSpinDegreesPerSecond,
            float maximumLifetimeSeconds,
            float solarDespawnRadius,
            float viewportMargin)
        {
            if (poolSize <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(poolSize),
                    poolSize,
                    "Comet pool size must be positive.");
            }

            RequireNonNegativeFinite(
                initialSpawnDelaySeconds,
                nameof(initialSpawnDelaySeconds));
            RequirePositiveRange(
                minimumSpawnIntervalSeconds,
                maximumSpawnIntervalSeconds,
                nameof(minimumSpawnIntervalSeconds),
                nameof(maximumSpawnIntervalSeconds));
            RequirePositiveFinite(orbitRadius, nameof(orbitRadius));
            RequirePositiveFinite(
                orbitPeriodSimulationDays,
                nameof(orbitPeriodSimulationDays));
            RequireFinite(
                orbitInclinationDegrees,
                nameof(orbitInclinationDegrees));
            RequireNonNegativeFinite(targetRadius, nameof(targetRadius));
            RequirePositiveRange(
                minimumSpeed,
                maximumSpeed,
                nameof(minimumSpeed),
                nameof(maximumSpeed));
            RequirePositiveRange(
                minimumRadius,
                maximumRadius,
                nameof(minimumRadius),
                nameof(maximumRadius));
            RequirePositiveRange(
                minimumSpinDegreesPerSecond,
                maximumSpinDegreesPerSecond,
                nameof(minimumSpinDegreesPerSecond),
                nameof(maximumSpinDegreesPerSecond));
            RequirePositiveFinite(
                maximumLifetimeSeconds,
                nameof(maximumLifetimeSeconds));
            RequirePositiveFinite(solarDespawnRadius, nameof(solarDespawnRadius));
            if (solarDespawnRadius <= orbitRadius)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(solarDespawnRadius),
                    solarDespawnRadius,
                    "Solar despawn radius must exceed the spawner orbit radius.");
            }

            RequireNonNegativeFinite(viewportMargin, nameof(viewportMargin));

            RandomSeed = randomSeed;
            PoolSize = poolSize;
            InitialSpawnDelaySeconds = initialSpawnDelaySeconds;
            MinimumSpawnIntervalSeconds = minimumSpawnIntervalSeconds;
            MaximumSpawnIntervalSeconds = maximumSpawnIntervalSeconds;
            OrbitRadius = orbitRadius;
            OrbitPeriodSimulationDays = orbitPeriodSimulationDays;
            OrbitInclinationDegrees = orbitInclinationDegrees;
            TargetRadius = targetRadius;
            MinimumSpeed = minimumSpeed;
            MaximumSpeed = maximumSpeed;
            MinimumRadius = minimumRadius;
            MaximumRadius = maximumRadius;
            MinimumSpinDegreesPerSecond = minimumSpinDegreesPerSecond;
            MaximumSpinDegreesPerSecond = maximumSpinDegreesPerSecond;
            MaximumLifetimeSeconds = maximumLifetimeSeconds;
            SolarDespawnRadius = solarDespawnRadius;
            ViewportMargin = viewportMargin;
        }

        /// <summary>Gets the deterministic pseudo-random seed.</summary>
        public int RandomSeed { get; }

        /// <summary>Gets the number of reusable comet instances.</summary>
        public int PoolSize { get; }

        /// <summary>Gets the first spawn delay in real seconds.</summary>
        public float InitialSpawnDelaySeconds { get; }

        /// <summary>Gets the minimum interval between spawn attempts.</summary>
        public float MinimumSpawnIntervalSeconds { get; }

        /// <summary>Gets the maximum interval between spawn attempts.</summary>
        public float MaximumSpawnIntervalSeconds { get; }

        /// <summary>Gets the circular presentation orbit radius.</summary>
        public float OrbitRadius { get; }

        /// <summary>Gets the presentation orbit period in simulated days.</summary>
        public float OrbitPeriodSimulationDays { get; }

        /// <summary>Gets the inclination of the spawner's presentation orbit.</summary>
        public float OrbitInclinationDegrees { get; }

        /// <summary>Gets the radius of the randomized target region around the Sun.</summary>
        public float TargetRadius { get; }

        /// <summary>Gets the minimum comet travel speed in presentation units per second.</summary>
        public float MinimumSpeed { get; }

        /// <summary>Gets the maximum comet travel speed in presentation units per second.</summary>
        public float MaximumSpeed { get; }

        /// <summary>Gets the minimum illustrative nucleus radius.</summary>
        public float MinimumRadius { get; }

        /// <summary>Gets the maximum illustrative nucleus radius.</summary>
        public float MaximumRadius { get; }

        /// <summary>Gets the minimum illustrative spin rate.</summary>
        public float MinimumSpinDegreesPerSecond { get; }

        /// <summary>Gets the maximum illustrative spin rate.</summary>
        public float MaximumSpinDegreesPerSecond { get; }

        /// <summary>Gets the maximum active lifetime used as a safety bound.</summary>
        public float MaximumLifetimeSeconds { get; }

        /// <summary>Gets the solar-system boundary after which offscreen comets despawn.</summary>
        public float SolarDespawnRadius { get; }

        /// <summary>Gets the normalized viewport margin used by culling.</summary>
        public float ViewportMargin { get; }

        private static void RequirePositiveRange(
            float minimum,
            float maximum,
            string minimumName,
            string maximumName)
        {
            RequirePositiveFinite(minimum, minimumName);
            RequirePositiveFinite(maximum, maximumName);
            if (maximum < minimum)
            {
                throw new ArgumentOutOfRangeException(
                    maximumName,
                    maximum,
                    $"{maximumName} must be greater than or equal to {minimumName}.");
            }
        }

        private static void RequirePositiveFinite(float value, string parameterName)
        {
            RequireFinite(value, parameterName);
            if (value <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    value,
                    "Value must be positive.");
            }
        }

        private static void RequireNonNegativeFinite(
            float value,
            string parameterName)
        {
            RequireFinite(value, parameterName);
            if (value < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    value,
                    "Value cannot be negative.");
            }
        }

        private static void RequireFinite(float value, string parameterName)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    value,
                    "Value must be finite.");
            }
        }
    }
}
