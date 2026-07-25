using System;

namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>Immutable presentation settings for optional celestial visual layers.</summary>
    public sealed class CelestialLayerVisualModel
    {
        /// <summary>Initializes validated visual-layer settings.</summary>
        public CelestialLayerVisualModel(
            string bodyStableId,
            float cloudShellRadiusMultiplier,
            float atmosphereShellRadiusMultiplier,
            float cloudRotationMultiplier)
            : this(
                bodyStableId,
                true,
                cloudShellRadiusMultiplier,
                atmosphereShellRadiusMultiplier,
                cloudRotationMultiplier,
                0f)
        {
        }

        /// <summary>Initializes validated visual-layer settings.</summary>
        public CelestialLayerVisualModel(
            string bodyStableId,
            bool hasCloudLayer,
            float cloudShellRadiusMultiplier,
            float atmosphereShellRadiusMultiplier,
            float cloudRotationMultiplier,
            float atmosphereCyclesPerRotation = 0f)
        {
            if (string.IsNullOrWhiteSpace(bodyStableId))
            {
                throw new ArgumentException(
                    "A layered visual model requires a stable body ID.",
                    nameof(bodyStableId));
            }

            if (!float.IsFinite(cloudShellRadiusMultiplier) ||
                cloudShellRadiusMultiplier <= 1f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(cloudShellRadiusMultiplier),
                    "Cloud shell radius must be finite and greater than the body radius.");
            }

            if (!float.IsFinite(atmosphereShellRadiusMultiplier) ||
                atmosphereShellRadiusMultiplier <= 1f ||
                (hasCloudLayer &&
                 atmosphereShellRadiusMultiplier <= cloudShellRadiusMultiplier))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(atmosphereShellRadiusMultiplier),
                    hasCloudLayer
                        ? "Atmosphere shell radius must be finite and exceed the cloud shell radius."
                        : "Atmosphere shell radius must be finite and exceed the body radius.");
            }

            if (!float.IsFinite(cloudRotationMultiplier) ||
                cloudRotationMultiplier <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(cloudRotationMultiplier),
                    "Cloud rotation multiplier must be finite and positive.");
            }

            if (!float.IsFinite(atmosphereCyclesPerRotation) ||
                atmosphereCyclesPerRotation < 0f ||
                atmosphereCyclesPerRotation > 1f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(atmosphereCyclesPerRotation),
                    "Atmosphere cycles per rotation must be finite and in [0, 1].");
            }

            BodyStableId = bodyStableId.Trim();
            HasCloudLayer = hasCloudLayer;
            CloudShellRadiusMultiplier = cloudShellRadiusMultiplier;
            AtmosphereShellRadiusMultiplier = atmosphereShellRadiusMultiplier;
            CloudRotationMultiplier = cloudRotationMultiplier;
            AtmosphereCyclesPerRotation = atmosphereCyclesPerRotation;
        }

        /// <summary>Gets the body that owns these layers.</summary>
        public string BodyStableId { get; }

        /// <summary>Gets whether the presentation includes a separate cloud shell.</summary>
        public bool HasCloudLayer { get; }

        /// <summary>Gets the cloud-shell radius relative to the physical body mesh.</summary>
        public float CloudShellRadiusMultiplier { get; }

        /// <summary>Gets the atmosphere-shell radius relative to the physical body mesh.</summary>
        public float AtmosphereShellRadiusMultiplier { get; }

        /// <summary>Gets total cloud rotation relative to the body's sidereal spin.</summary>
        public float CloudRotationMultiplier { get; }

        /// <summary>Gets atmosphere-detail cycles per signed body rotation.</summary>
        public float AtmosphereCyclesPerRotation { get; }

        /// <summary>Evaluates a deterministic atmosphere phase from absolute simulation time.</summary>
        public float EvaluateAtmospherePhase(
            double simulationTimeSeconds,
            double signedRotationPeriodSeconds)
        {
            if (!IsFinite(simulationTimeSeconds))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(simulationTimeSeconds),
                    simulationTimeSeconds,
                    "Simulation time must be finite.");
            }

            if (!IsFinite(signedRotationPeriodSeconds) ||
                Math.Abs(signedRotationPeriodSeconds) < double.Epsilon)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(signedRotationPeriodSeconds),
                    signedRotationPeriodSeconds,
                    "Rotation period must be finite and non-zero.");
            }

            double cycles =
                simulationTimeSeconds /
                signedRotationPeriodSeconds *
                AtmosphereCyclesPerRotation;
            return (float)(cycles - Math.Floor(cycles));
        }

        private static bool IsFinite(double value) =>
            !double.IsNaN(value) && !double.IsInfinity(value);
    }
}
