using System;

namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>Immutable deterministic parameters for one ice-giant treatment.</summary>
    public sealed class IceGiantVisualModel
    {
        /// <summary>Creates a validated ice-giant visual model.</summary>
        public IceGiantVisualModel(
            string bodyStableId,
            float atmosphereShellRadiusMultiplier,
            float detailCyclesPerRotation)
        {
            BodyStableId = string.IsNullOrWhiteSpace(bodyStableId)
                ? throw new ArgumentException(
                    "An ice-giant visual requires a stable body ID.",
                    nameof(bodyStableId))
                : bodyStableId.Trim();
            AtmosphereShellRadiusMultiplier = RequireRange(
                atmosphereShellRadiusMultiplier,
                1f,
                1.2f,
                nameof(atmosphereShellRadiusMultiplier));
            DetailCyclesPerRotation = RequireRange(
                detailCyclesPerRotation,
                0f,
                1f,
                nameof(detailCyclesPerRotation));
        }

        /// <summary>Gets the body stable ID this treatment belongs to.</summary>
        public string BodyStableId { get; }

        /// <summary>Gets the atmosphere radius relative to the physical surface.</summary>
        public float AtmosphereShellRadiusMultiplier { get; }

        /// <summary>Gets presentation-detail cycles per signed body rotation.</summary>
        public float DetailCyclesPerRotation { get; }

        /// <summary>
        /// Evaluates a wrapping, deterministic presentation-detail phase.
        /// </summary>
        public float EvaluateDetailPhase(
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

            double signedCycles =
                simulationTimeSeconds /
                signedRotationPeriodSeconds *
                DetailCyclesPerRotation;
            double phase = signedCycles - Math.Floor(signedCycles);
            return (float)phase;
        }

        private static float RequireRange(
            float value,
            float exclusiveMinimum,
            float inclusiveMaximum,
            string parameterName)
        {
            if (!float.IsFinite(value) ||
                value <= exclusiveMinimum ||
                value > inclusiveMaximum)
            {
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    value,
                    $"Value must be greater than {exclusiveMinimum} and at most {inclusiveMaximum}.");
            }

            return value;
        }

        private static bool IsFinite(double value) =>
            !double.IsNaN(value) && !double.IsInfinity(value);
    }
}
