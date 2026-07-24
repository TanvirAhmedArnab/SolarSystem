using System;

namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>Immutable deterministic parameters for one solar hero treatment.</summary>
    public sealed class SolarVisualModel
    {
        /// <summary>Creates a validated solar visual model.</summary>
        public SolarVisualModel(
            string bodyStableId,
            float coronaShellRadiusMultiplier,
            float surfaceFlowCyclesPerRotation,
            float coronaFlowCyclesPerRotation)
        {
            BodyStableId = string.IsNullOrWhiteSpace(bodyStableId)
                ? throw new ArgumentException(
                    "A solar visual requires a stable body ID.",
                    nameof(bodyStableId))
                : bodyStableId.Trim();
            CoronaShellRadiusMultiplier = RequireRange(
                coronaShellRadiusMultiplier,
                1f,
                2f,
                nameof(coronaShellRadiusMultiplier));
            SurfaceFlowCyclesPerRotation = RequireRange(
                surfaceFlowCyclesPerRotation,
                0f,
                4f,
                nameof(surfaceFlowCyclesPerRotation));
            CoronaFlowCyclesPerRotation = RequireRange(
                coronaFlowCyclesPerRotation,
                0f,
                4f,
                nameof(coronaFlowCyclesPerRotation));
        }

        /// <summary>Gets the body stable ID this treatment belongs to.</summary>
        public string BodyStableId { get; }

        /// <summary>Gets the corona radius relative to the physical surface.</summary>
        public float CoronaShellRadiusMultiplier { get; }

        /// <summary>Gets surface-flow cycles per signed body rotation.</summary>
        public float SurfaceFlowCyclesPerRotation { get; }

        /// <summary>Gets corona-flow cycles per signed body rotation.</summary>
        public float CoronaFlowCyclesPerRotation { get; }

        /// <summary>Evaluates a smoothly wrapping surface phase from absolute simulation time.</summary>
        public float EvaluateSurfacePhase(
            double simulationTimeSeconds,
            double signedRotationPeriodSeconds)
        {
            return EvaluatePhase(
                simulationTimeSeconds,
                signedRotationPeriodSeconds,
                SurfaceFlowCyclesPerRotation);
        }

        /// <summary>Evaluates a smoothly wrapping corona phase from absolute simulation time.</summary>
        public float EvaluateCoronaPhase(
            double simulationTimeSeconds,
            double signedRotationPeriodSeconds)
        {
            return EvaluatePhase(
                simulationTimeSeconds,
                signedRotationPeriodSeconds,
                CoronaFlowCyclesPerRotation);
        }

        private static float EvaluatePhase(
            double simulationTimeSeconds,
            double signedRotationPeriodSeconds,
            double cyclesPerRotation)
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
                cyclesPerRotation;
            double phase = signedCycles - Math.Floor(signedCycles);
            return (float)phase;
        }

        private static float RequireRange(
            float value,
            float exclusiveMinimum,
            float inclusiveMaximum,
            string parameterName)
        {
            if (float.IsNaN(value) ||
                float.IsInfinity(value) ||
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
