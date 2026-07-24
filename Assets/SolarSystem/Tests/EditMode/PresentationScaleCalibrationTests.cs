using System;
using System.Collections.Generic;
using NUnit.Framework;
using Tanvir.SolarSystem.Application;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Mathematics;
using Tanvir.SolarSystem.Presentation.Scale;
using Tanvir.SolarSystem.Simulation;
using UnityEditor;
using UnityEngine;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class PresentationScaleCalibrationTests
    {
        private const string CatalogPath =
            "Assets/SolarSystem/Content/Data/Catalog_SolarSystem.asset";
        private const string ScalePath =
            "Assets/SolarSystem/Content/Data/Scale/Scale_PresentationGraybox.asset";
        private const int SynodicCycleSamples = 4096;

        private static readonly string[] PlanetOrder =
        {
            "mercury",
            "venus",
            "earth",
            "mars",
            "jupiter",
            "saturn",
            "uranus",
            "neptune"
        };

        private CelestialCatalog catalog;
        private CelestialScaleProjector projector;
        private KeplerOrbitEvaluator evaluator;

        [SetUp]
        public void SetUp()
        {
            CelestialCatalogDefinition catalogDefinition =
                AssetDatabase.LoadAssetAtPath<CelestialCatalogDefinition>(CatalogPath);
            PresentationScaleDefinition scaleDefinition =
                AssetDatabase.LoadAssetAtPath<PresentationScaleDefinition>(ScalePath);
            Assert.That(catalogDefinition, Is.Not.Null);
            Assert.That(scaleDefinition, Is.Not.Null);

            catalog = catalogDefinition.BuildCatalog();
            projector = new CelestialScaleProjector(scaleDefinition.ToParameters());
            evaluator = new KeplerOrbitEvaluator();
        }

        [Test]
        public void EveryBodyRadius_IsProportionalToEarthMeanRadius()
        {
            CelestialBodyModel earth = Find("earth");
            Assert.That(
                earth.MeanRadiusKm,
                Is.EqualTo(CelestialReferenceUnits.EarthMeanRadiusKm)
                    .Within(0.000001d));
            Assert.That(
                projector.ProjectRadius(earth.MeanRadiusKm),
                Is.EqualTo(ReadableOverviewScaleContract.EarthDisplayRadius)
                    .Within(0.000001d));

            foreach (CelestialBodyModel body in catalog.OrderedBodies)
            {
                double expected =
                    body.MeanRadiusKm / CelestialReferenceUnits.EarthMeanRadiusKm;
                Assert.That(
                    projector.ProjectRadius(body.MeanRadiusKm),
                    Is.EqualTo(expected).Within(0.0001d),
                    $"{body.DisplayName} must use the same Earth-radius ratio.");
            }
        }

        [Test]
        public void NormalizedOrbitUnit_MatchesAuthoredJplMercuryVenusEnvelope()
        {
            CelestialBodyModel mercury = Find("mercury");
            CelestialBodyModel venus = Find("venus");
            double mercuryApoapsis =
                mercury.Orbit.Value.SemiMajorAxisKm *
                (1d + mercury.Orbit.Value.Eccentricity);
            double venusPeriapsis =
                venus.Orbit.Value.SemiMajorAxisKm *
                (1d - venus.Orbit.Value.Eccentricity);

            Assert.That(
                venusPeriapsis - mercuryApoapsis,
                Is.EqualTo(
                    GuidedScaleComparisonContract.MercuryVenusEnvelopeGapKm)
                    .Within(0.001d));
        }

        [Test]
        public void AdjacentPlanetPairs_RemainClearAcrossCompleteSynodicCycles()
        {
            for (int pairIndex = 0; pairIndex < PlanetOrder.Length - 1; pairIndex++)
            {
                CelestialBodyModel inner = Find(PlanetOrder[pairIndex]);
                CelestialBodyModel outer = Find(PlanetOrder[pairIndex + 1]);
                double conservativeClearance =
                    EvaluateConservativeEnvelopeClearance(inner, outer);
                Assert.That(
                    conservativeClearance,
                    Is.GreaterThanOrEqualTo(projector.MinimumSurfaceClearance),
                    $"{inner.DisplayName}-{outer.DisplayName} orbit envelopes violate " +
                    "the minimum readable clearance.");

                double synodicPeriod = CalculateSynodicPeriod(inner, outer);
                double sampledMinimum = double.MaxValue;
                for (int sample = 0; sample <= SynodicCycleSamples; sample++)
                {
                    double timeSeconds =
                        synodicPeriod * sample / SynodicCycleSamples;
                    sampledMinimum = Math.Min(
                        sampledMinimum,
                        EvaluateSurfaceClearance(inner, outer, timeSeconds));
                }

                Assert.That(
                    sampledMinimum,
                    Is.GreaterThanOrEqualTo(projector.MinimumSurfaceClearance),
                    $"{inner.DisplayName}-{outer.DisplayName} failed its complete " +
                    "deterministic synodic-cycle sample.");
            }
        }

        [Test]
        public void SunMercuryAndEarthMoon_HierarchyMaintainsReadableClearance()
        {
            CelestialBodyModel sun = Find("sun");
            CelestialBodyModel mercury = Find("mercury");
            CelestialBodyModel earth = Find("earth");
            CelestialBodyModel moon = Find("moon");

            double mercuryPeriapsis =
                mercury.Orbit.Value.SemiMajorAxisKm *
                (1d - mercury.Orbit.Value.Eccentricity);
            double sunMercuryClearance =
                ProjectDistance(mercuryPeriapsis) -
                EffectiveRadius(sun) -
                EffectiveRadius(mercury);
            Assert.That(
                sunMercuryClearance,
                Is.GreaterThanOrEqualTo(projector.MinimumSurfaceClearance));

            double sampledMinimum = double.MaxValue;
            double lunarPeriod = moon.Orbit.Value.OrbitalPeriodSeconds;
            for (int sample = 0; sample <= SynodicCycleSamples; sample++)
            {
                double timeSeconds = lunarPeriod * sample / SynodicCycleSamples;
                CelestialState moonState =
                    evaluator.Evaluate(moon, Double3.Zero, timeSeconds);
                double centerDistance = projector.ProjectRelativePosition(
                    moonState.ParentRelativePositionKm).magnitude;
                sampledMinimum = Math.Min(
                    sampledMinimum,
                    centerDistance - EffectiveRadius(earth) - EffectiveRadius(moon));
            }

            Assert.That(
                sampledMinimum,
                Is.GreaterThanOrEqualTo(projector.MinimumSurfaceClearance));
        }

        [Test]
        public void SignedSiderealRates_UseEarthRotationReferenceAndCorrectDirection()
        {
            CelestialBodyModel earth = Find("earth");
            Assert.That(
                Math.Abs(earth.RotationPeriodSeconds),
                Is.EqualTo(SimulationTimeControlService.BaselineSecondsPerRealSecond)
                    .Within(0.000001d));

            double simulationTime =
                SimulationTimeControlService.BaselineSecondsPerRealSecond * 0.1d;
            foreach (CelestialBodyModel body in catalog.OrderedBodies)
            {
                CelestialState state =
                    evaluator.Evaluate(body, Double3.Zero, simulationTime);
                double expected = NormalizeDegrees(
                    360d * simulationTime / body.RotationPeriodSeconds);
                Assert.That(
                    state.RotationAngleDeg,
                    Is.EqualTo(expected).Within(0.000000001d),
                    $"{body.DisplayName} must derive spin from its signed sidereal period.");

                bool expectedRetrograde =
                    body.Id.Value == "venus" || body.Id.Value == "uranus";
                Assert.That(
                    body.RotationPeriodSeconds < 0d,
                    Is.EqualTo(expectedRetrograde),
                    $"{body.DisplayName} has an unexpected rotation direction.");
            }
        }

        [Test]
        public void GuidedPhysicalReference_ExposesWhyLiteralScaleNeedsAControlledMode()
        {
            PhysicalScaleReference reference = projector.PhysicalReference;
            CelestialBodyModel earth = Find("earth");
            CelestialBodyModel moon = Find("moon");

            Assert.That(
                reference.ToDisplayUnits(earth.MeanRadiusKm),
                Is.EqualTo(1d).Within(0.000000001d));
            Assert.That(
                reference.ToDisplayUnits(moon.MeanRadiusKm),
                Is.EqualTo(moon.MeanRadiusKm / earth.MeanRadiusKm)
                    .Within(0.000000001d));
            Assert.That(
                reference.ToDisplayUnits(moon.Orbit.Value.SemiMajorAxisKm),
                Is.EqualTo(
                    moon.Orbit.Value.SemiMajorAxisKm / earth.MeanRadiusKm)
                    .Within(0.000000001d));
            Assert.That(
                reference.ToDisplayUnits(earth.Orbit.Value.SemiMajorAxisKm),
                Is.GreaterThan(23000d));
        }

        private double EvaluateConservativeEnvelopeClearance(
            CelestialBodyModel inner,
            CelestialBodyModel outer)
        {
            OrbitalElements innerOrbit = inner.Orbit.Value;
            OrbitalElements outerOrbit = outer.Orbit.Value;
            double innerApoapsis =
                innerOrbit.SemiMajorAxisKm * (1d + innerOrbit.Eccentricity);
            double outerPeriapsis =
                outerOrbit.SemiMajorAxisKm * (1d - outerOrbit.Eccentricity);
            return
                ProjectDistance(outerPeriapsis) -
                ProjectDistance(innerApoapsis) -
                EffectiveRadius(inner) -
                EffectiveRadius(outer);
        }

        private double EvaluateSurfaceClearance(
            CelestialBodyModel inner,
            CelestialBodyModel outer,
            double timeSeconds)
        {
            CelestialState innerState =
                evaluator.Evaluate(inner, Double3.Zero, timeSeconds);
            CelestialState outerState =
                evaluator.Evaluate(outer, Double3.Zero, timeSeconds);
            Vector3 innerPosition =
                projector.ProjectRelativePosition(innerState.ParentRelativePositionKm);
            Vector3 outerPosition =
                projector.ProjectRelativePosition(outerState.ParentRelativePositionKm);
            return
                Vector3.Distance(innerPosition, outerPosition) -
                EffectiveRadius(inner) -
                EffectiveRadius(outer);
        }

        private double EffectiveRadius(CelestialBodyModel body)
        {
            double radius = projector.ProjectRadius(body.MeanRadiusKm);
            return body.Id.Value == "saturn"
                ? radius * ReadableOverviewScaleContract.SaturnRingOuterRadiusMultiplier
                : radius;
        }

        private float ProjectDistance(double physicalDistanceKm)
        {
            return projector.ProjectRelativePosition(
                new Double3(physicalDistanceKm, 0d, 0d)).magnitude;
        }

        private static double CalculateSynodicPeriod(
            CelestialBodyModel inner,
            CelestialBodyModel outer)
        {
            double innerPeriod = inner.Orbit.Value.OrbitalPeriodSeconds;
            double outerPeriod = outer.Orbit.Value.OrbitalPeriodSeconds;
            return 1d / Math.Abs((1d / innerPeriod) - (1d / outerPeriod));
        }

        private CelestialBodyModel Find(string stableId)
        {
            foreach (CelestialBodyModel body in catalog.OrderedBodies)
            {
                if (body.Id.Value == stableId)
                {
                    return body;
                }
            }

            throw new KeyNotFoundException(stableId);
        }

        private static double NormalizeDegrees(double degrees)
        {
            double normalized = degrees % 360d;
            return normalized < 0d ? normalized + 360d : normalized;
        }
    }
}
