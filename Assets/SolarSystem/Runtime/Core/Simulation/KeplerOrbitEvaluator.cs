using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Tanvir.SolarSystem.Mathematics;

namespace Tanvir.SolarSystem.Simulation
{
    /// <summary>Evaluates deterministic elliptical orbits and signed sidereal rotation from authoritative time.</summary>
    public sealed class KeplerOrbitEvaluator
    {
        /// <summary>The default maximum Newton-Raphson iteration count.</summary>
        public const int DefaultMaxIterations = 20;

        /// <summary>The default eccentric-anomaly convergence tolerance in radians.</summary>
        public const double DefaultToleranceRadians = 1e-12d;

        private const double TwoPi = 2d * Math.PI;
        private const double DegreesPerRotation = 360d;
        private const double DegreesToRadians = Math.PI / 180d;

        private readonly int maxIterations;
        private readonly double toleranceRadians;

        /// <summary>Initializes an evaluator with explicit numerical convergence settings.</summary>
        public KeplerOrbitEvaluator(
            int maxIterations = DefaultMaxIterations,
            double toleranceRadians = DefaultToleranceRadians)
        {
            if (maxIterations <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxIterations),
                    maxIterations,
                    "Maximum iterations must be positive.");
            }

            if (!IsFinite(toleranceRadians) || toleranceRadians <= 0d)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(toleranceRadians),
                    toleranceRadians,
                    "Tolerance must be positive and finite.");
            }

            this.maxIterations = maxIterations;
            this.toleranceRadians = toleranceRadians;
        }

        /// <summary>Evaluates an entire catalog in its deterministic parent-first order.</summary>
        public IReadOnlyList<CelestialState> Evaluate(CelestialCatalog catalog, double simulationTimeSeconds)
        {
            if (catalog == null)
            {
                throw new ArgumentNullException(nameof(catalog));
            }

            var orderedStates = new CelestialState[catalog.Count];
            Evaluate(catalog, simulationTimeSeconds, orderedStates);
            return new ReadOnlyCollection<CelestialState>(orderedStates);
        }

        /// <summary>Evaluates a catalog into a caller-owned buffer without per-call collections.</summary>
        public void Evaluate(
            CelestialCatalog catalog,
            double simulationTimeSeconds,
            CelestialState[] destination)
        {
            if (catalog == null)
            {
                throw new ArgumentNullException(nameof(catalog));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (destination.Length < catalog.Count)
            {
                throw new ArgumentException(
                    "Destination buffer must contain at least one element per catalog body.",
                    nameof(destination));
            }

            RequireFiniteTime(simulationTimeSeconds);
            for (int index = 0; index < catalog.Count; index++)
            {
                CelestialBodyModel body = catalog.OrderedBodies[index];
                Double3 parentPositionKm = Double3.Zero;
                if (body.ParentId.HasValue)
                {
                    int parentIndex = FindPriorBodyIndex(catalog, body.ParentId.Value, index);
                    parentPositionKm = destination[parentIndex].PhysicalPositionKm;
                }

                destination[index] = Evaluate(body, parentPositionKm, simulationTimeSeconds);
            }
        }

        /// <summary>Evaluates one body from authoritative time and its parent's world position.</summary>
        public CelestialState Evaluate(
            CelestialBodyModel body,
            Double3 parentPhysicalPositionKm,
            double simulationTimeSeconds)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (!parentPhysicalPositionKm.IsFinite)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(parentPhysicalPositionKm),
                    "Parent position must be finite.");
            }

            RequireFiniteTime(simulationTimeSeconds);
            double rotationAngleDeg = EvaluateRotationAngleDeg(body.RotationPeriodSeconds, simulationTimeSeconds);

            if (!body.Orbit.HasValue)
            {
                return new CelestialState(
                    body.Id,
                    Double3.Zero,
                    parentPhysicalPositionKm,
                    0d,
                    rotationAngleDeg);
            }

            OrbitalElements orbit = body.Orbit.Value;
            double meanMotionRadiansPerSecond = TwoPi / orbit.OrbitalPeriodSeconds;
            double meanAnomalyRadians = NormalizeRadians(
                (orbit.MeanAnomalyAtEpochDeg * DegreesToRadians) +
                (meanMotionRadiansPerSecond * simulationTimeSeconds));
            double eccentricAnomalyRadians = SolveEccentricAnomaly(meanAnomalyRadians, orbit.Eccentricity);

            double cosineE = Math.Cos(eccentricAnomalyRadians);
            double sineE = Math.Sin(eccentricAnomalyRadians);
            double eccentricityTerm = Math.Sqrt(1d - (orbit.Eccentricity * orbit.Eccentricity));
            double orbitalXKm = orbit.SemiMajorAxisKm * (cosineE - orbit.Eccentricity);
            double orbitalYKm = orbit.SemiMajorAxisKm * eccentricityTerm * sineE;
            Double3 relativePositionKm = RotateOrbitalPlaneToReference(
                orbitalXKm,
                orbitalYKm,
                orbit.ArgumentPeriapsisDeg * DegreesToRadians,
                orbit.InclinationDeg * DegreesToRadians,
                orbit.LongitudeAscendingNodeDeg * DegreesToRadians);

            double eccentricAnomalyRate =
                meanMotionRadiansPerSecond / (1d - (orbit.Eccentricity * cosineE));
            double velocityXKmPerSec = -orbit.SemiMajorAxisKm * sineE * eccentricAnomalyRate;
            double velocityYKmPerSec =
                orbit.SemiMajorAxisKm * eccentricityTerm * cosineE * eccentricAnomalyRate;
            double orbitalSpeedKmPerSec = Math.Sqrt(
                (velocityXKmPerSec * velocityXKmPerSec) +
                (velocityYKmPerSec * velocityYKmPerSec));

            return new CelestialState(
                body.Id,
                relativePositionKm,
                parentPhysicalPositionKm + relativePositionKm,
                orbitalSpeedKmPerSec,
                rotationAngleDeg);
        }

        /// <summary>Solves Kepler's equation for eccentric anomaly in radians.</summary>
        public double SolveEccentricAnomaly(double meanAnomalyRadians, double eccentricity)
        {
            if (!IsFinite(meanAnomalyRadians))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(meanAnomalyRadians),
                    meanAnomalyRadians,
                    "Mean anomaly must be finite.");
            }

            if (!IsFinite(eccentricity) || eccentricity < 0d || eccentricity >= 1d)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(eccentricity),
                    eccentricity,
                    "Elliptical eccentricity must be in [0, 1).");
            }

            double normalizedMeanAnomaly = NormalizeRadians(meanAnomalyRadians);
            double eccentricAnomaly = eccentricity < 0.8d ? normalizedMeanAnomaly : Math.PI;

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                double function =
                    eccentricAnomaly - (eccentricity * Math.Sin(eccentricAnomaly)) - normalizedMeanAnomaly;
                double derivative = 1d - (eccentricity * Math.Cos(eccentricAnomaly));
                double correction = function / derivative;
                eccentricAnomaly -= correction;

                if (Math.Abs(correction) <= toleranceRadians)
                {
                    return NormalizeRadians(eccentricAnomaly);
                }
            }

            throw new InvalidOperationException(
                $"Kepler solver did not converge in {maxIterations} iterations for eccentricity {eccentricity:R}.");
        }

        private static Double3 RotateOrbitalPlaneToReference(
            double orbitalXKm,
            double orbitalYKm,
            double argumentPeriapsisRadians,
            double inclinationRadians,
            double ascendingNodeRadians)
        {
            double cosinePeriapsis = Math.Cos(argumentPeriapsisRadians);
            double sinePeriapsis = Math.Sin(argumentPeriapsisRadians);
            double periapsisX = (cosinePeriapsis * orbitalXKm) - (sinePeriapsis * orbitalYKm);
            double periapsisY = (sinePeriapsis * orbitalXKm) + (cosinePeriapsis * orbitalYKm);

            double inclinedX = periapsisX;
            double inclinedY = Math.Cos(inclinationRadians) * periapsisY;
            double inclinedZ = Math.Sin(inclinationRadians) * periapsisY;

            double cosineNode = Math.Cos(ascendingNodeRadians);
            double sineNode = Math.Sin(ascendingNodeRadians);
            return new Double3(
                (cosineNode * inclinedX) - (sineNode * inclinedY),
                (sineNode * inclinedX) + (cosineNode * inclinedY),
                inclinedZ);
        }

        private static int FindPriorBodyIndex(
            CelestialCatalog catalog,
            CelestialBodyId parentId,
            int childIndex)
        {
            for (int index = 0; index < childIndex; index++)
            {
                if (catalog.OrderedBodies[index].Id == parentId)
                {
                    return index;
                }
            }

            throw new InvalidOperationException(
                $"Catalog order placed a body before parent '{parentId}'.");
        }

        private static double EvaluateRotationAngleDeg(
            double signedRotationPeriodSeconds,
            double simulationTimeSeconds)
        {
            double angle = DegreesPerRotation * simulationTimeSeconds / signedRotationPeriodSeconds;
            if (!IsFinite(angle))
            {
                throw new InvalidOperationException("Rotation evaluation exceeded its finite range.");
            }

            double normalized = angle % DegreesPerRotation;
            return normalized < 0d ? normalized + DegreesPerRotation : normalized;
        }

        private static double NormalizeRadians(double radians)
        {
            if (!IsFinite(radians))
            {
                throw new InvalidOperationException("Orbital anomaly exceeded its finite range.");
            }

            double normalized = radians % TwoPi;
            return normalized < 0d ? normalized + TwoPi : normalized;
        }

        private static void RequireFiniteTime(double simulationTimeSeconds)
        {
            if (!IsFinite(simulationTimeSeconds))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(simulationTimeSeconds),
                    simulationTimeSeconds,
                    "Simulation time must be finite.");
            }
        }

        private static bool IsFinite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
    }
}
