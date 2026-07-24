using System;

namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>Immutable deterministic parameters for one gas-giant treatment.</summary>
    public sealed class GasGiantVisualModel
    {
        /// <summary>Creates a validated gas-giant visual model.</summary>
        public GasGiantVisualModel(
            string bodyStableId,
            float atmosphereShellRadiusMultiplier,
            float bandFlowCyclesPerRotation)
        {
            BodyStableId = string.IsNullOrWhiteSpace(bodyStableId)
                ? throw new ArgumentException(
                    "A gas-giant visual requires a stable body ID.",
                    nameof(bodyStableId))
                : bodyStableId.Trim();
            AtmosphereShellRadiusMultiplier = RequireRange(
                atmosphereShellRadiusMultiplier,
                1f,
                1.2f,
                nameof(atmosphereShellRadiusMultiplier));
            BandFlowCyclesPerRotation = RequireRange(
                bandFlowCyclesPerRotation,
                0f,
                1f,
                nameof(bandFlowCyclesPerRotation));
        }

        /// <summary>Gets the body stable ID this treatment belongs to.</summary>
        public string BodyStableId { get; }

        /// <summary>Gets the atmosphere radius relative to the physical surface.</summary>
        public float AtmosphereShellRadiusMultiplier { get; }

        /// <summary>Gets the band-detail flow cycles per signed body rotation.</summary>
        public float BandFlowCyclesPerRotation { get; }

        /// <summary>Evaluates a smoothly wrapping band phase from absolute simulation time.</summary>
        public float EvaluateBandPhase(
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
                BandFlowCyclesPerRotation;
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
