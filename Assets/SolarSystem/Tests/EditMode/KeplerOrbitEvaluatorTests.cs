using System;
using NUnit.Framework;
using Tanvir.SolarSystem.Mathematics;
using Tanvir.SolarSystem.Simulation;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class KeplerOrbitEvaluatorTests
    {
        private const double PositionToleranceKm = 1e-9d;
        private const double SpeedToleranceKmPerSec = 1e-12d;
        private const double AngleToleranceDeg = 1e-9d;

        private readonly KeplerOrbitEvaluator evaluator = new KeplerOrbitEvaluator();

        [Test]
        public void Evaluate_CircularOrbitAtQuarterPeriod_ReturnsPositiveOrbitalY()
        {
            CelestialBodyModel body = CelestialTestFactory.CreateOrbitingBody("earth", "sun");

            CelestialState state = evaluator.Evaluate(body, Double3.Zero, simulationTimeSeconds: 10d);

            Assert.That(state.ParentRelativePositionKm.X, Is.EqualTo(0d).Within(PositionToleranceKm));
            Assert.That(state.ParentRelativePositionKm.Y, Is.EqualTo(100d).Within(PositionToleranceKm));
            Assert.That(state.ParentRelativePositionKm.Z, Is.EqualTo(0d).Within(PositionToleranceKm));
        }

        [Test]
        public void Evaluate_EccentricOrbitAtEpoch_ReturnsPeriapsisDistance()
        {
            CelestialBodyModel body = CelestialTestFactory.CreateOrbitingBody(
                "earth",
                "sun",
                semiMajorAxisKm: 100d,
                eccentricity: 0.2d);

            CelestialState state = evaluator.Evaluate(body, Double3.Zero, simulationTimeSeconds: 0d);

            Assert.That(state.ParentRelativePositionKm.X, Is.EqualTo(80d).Within(PositionToleranceKm));
            Assert.That(state.ParentRelativePositionKm.Y, Is.EqualTo(0d).Within(PositionToleranceKm));
        }

        [Test]
        public void Evaluate_NinetyDegreeInclination_MapsQuarterOrbitToPositiveZ()
        {
            CelestialBodyModel body = CelestialTestFactory.CreateOrbitingBody(
                "earth",
                "sun",
                inclinationDeg: 90d);

            CelestialState state = evaluator.Evaluate(body, Double3.Zero, simulationTimeSeconds: 10d);

            Assert.That(state.ParentRelativePositionKm.X, Is.EqualTo(0d).Within(PositionToleranceKm));
            Assert.That(state.ParentRelativePositionKm.Y, Is.EqualTo(0d).Within(PositionToleranceKm));
            Assert.That(state.ParentRelativePositionKm.Z, Is.EqualTo(100d).Within(PositionToleranceKm));
        }

        [Test]
        public void Evaluate_NinetyDegreeAscendingNode_RotatesPeriapsisToPositiveY()
        {
            CelestialBodyModel body = CelestialTestFactory.CreateOrbitingBody(
                "earth",
                "sun",
                longitudeAscendingNodeDeg: 90d);

            CelestialState state = evaluator.Evaluate(body, Double3.Zero, simulationTimeSeconds: 0d);

            Assert.That(state.ParentRelativePositionKm.X, Is.EqualTo(0d).Within(PositionToleranceKm));
            Assert.That(state.ParentRelativePositionKm.Y, Is.EqualTo(100d).Within(PositionToleranceKm));
        }

        [Test]
        public void Evaluate_CircularOrbit_ReturnsAnalyticalSpeed()
        {
            CelestialBodyModel body = CelestialTestFactory.CreateOrbitingBody(
                "earth",
                "sun",
                semiMajorAxisKm: 100d,
                orbitalPeriodSeconds: 40d);
            double expectedSpeed = (2d * Math.PI * 100d) / 40d;

            CelestialState state = evaluator.Evaluate(body, Double3.Zero, simulationTimeSeconds: 7d);

            Assert.That(
                state.OrbitalSpeedKmPerSec,
                Is.EqualTo(expectedSpeed).Within(SpeedToleranceKmPerSec));
        }

        [Test]
        public void EvaluateCatalog_ParentAndMoon_ComposesWorldPosition()
        {
            CelestialBodyModel sun = CelestialTestFactory.CreateSun();
            CelestialBodyModel earth = CelestialTestFactory.CreateOrbitingBody(
                "earth",
                "sun",
                semiMajorAxisKm: 100d);
            CelestialBodyModel moon = CelestialTestFactory.CreateOrbitingBody(
                "moon",
                "earth",
                CelestialBodyCategory.Moon,
                semiMajorAxisKm: 10d,
                orbitalPeriodSeconds: 4d);
            CelestialCatalog catalog = CelestialTestFactory.BuildCatalog(moon, sun, earth);

            var states = evaluator.Evaluate(catalog, simulationTimeSeconds: 0d);

            Assert.That(states[2].Id, Is.EqualTo(new CelestialBodyId("moon")));
            Assert.That(states[2].ParentRelativePositionKm.X, Is.EqualTo(10d).Within(PositionToleranceKm));
            Assert.That(states[2].PhysicalPositionKm.X, Is.EqualTo(110d).Within(PositionToleranceKm));
        }

        [Test]
        public void Evaluate_SameCatalogAndTime_ReturnsExactlyRepeatableState()
        {
            CelestialCatalog catalog = CelestialTestFactory.BuildCatalog(
                CelestialTestFactory.CreateSun(),
                CelestialTestFactory.CreateOrbitingBody(
                    "earth",
                    "sun",
                    eccentricity: 0.0167d,
                    inclinationDeg: 0.00005d,
                    longitudeAscendingNodeDeg: -11.26064d,
                    argumentPeriapsisDeg: 114.20783d,
                    meanAnomalyAtEpochDeg: 358.617d));

            var first = evaluator.Evaluate(catalog, 123_456.789d);
            var second = evaluator.Evaluate(catalog, 123_456.789d);

            Assert.That(second[1].ParentRelativePositionKm, Is.EqualTo(first[1].ParentRelativePositionKm));
            Assert.That(second[1].PhysicalPositionKm, Is.EqualTo(first[1].PhysicalPositionKm));
            Assert.That(second[1].OrbitalSpeedKmPerSec, Is.EqualTo(first[1].OrbitalSpeedKmPerSec));
            Assert.That(second[1].RotationAngleDeg, Is.EqualTo(first[1].RotationAngleDeg));
        }

        [Test]
        public void Evaluate_RetrogradeRotationAtQuarterPeriod_Returns270Degrees()
        {
            CelestialBodyModel body = CelestialTestFactory.CreateOrbitingBody(
                "venus",
                "sun",
                rotationPeriodSeconds: -40d);

            CelestialState state = evaluator.Evaluate(body, Double3.Zero, simulationTimeSeconds: 10d);

            Assert.That(state.RotationAngleDeg, Is.EqualTo(270d).Within(AngleToleranceDeg));
        }

        [Test]
        public void SolveEccentricAnomaly_HighEccentricity_SatisfiesKeplerEquation()
        {
            const double meanAnomaly = 2.1d;
            const double eccentricity = 0.9d;

            double eccentricAnomaly = evaluator.SolveEccentricAnomaly(meanAnomaly, eccentricity);
            double residual = eccentricAnomaly - (eccentricity * Math.Sin(eccentricAnomaly)) - meanAnomaly;

            Assert.That(residual, Is.EqualTo(0d).Within(KeplerOrbitEvaluator.DefaultToleranceRadians));
        }

        [Test]
        public void Evaluate_RootBody_RemainsAtProvidedOrigin()
        {
            var origin = new Double3(10d, 20d, 30d);

            CelestialState state = evaluator.Evaluate(
                CelestialTestFactory.CreateSun(),
                origin,
                simulationTimeSeconds: 25d);

            Assert.That(state.ParentRelativePositionKm, Is.EqualTo(Double3.Zero));
            Assert.That(state.PhysicalPositionKm, Is.EqualTo(origin));
            Assert.That(state.OrbitalSpeedKmPerSec, Is.Zero);
        }

        [TestCase(-0.1d)]
        [TestCase(1d)]
        public void SolveEccentricAnomaly_NonEllipticalEccentricity_Throws(double eccentricity)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => evaluator.SolveEccentricAnomaly(0d, eccentricity));
        }

        [Test]
        public void Evaluate_NonFiniteTime_Throws()
        {
            CelestialBodyModel body = CelestialTestFactory.CreateOrbitingBody("earth", "sun");

            Assert.Throws<ArgumentOutOfRangeException>(
                () => evaluator.Evaluate(body, Double3.Zero, double.PositiveInfinity));
        }
    }
}
