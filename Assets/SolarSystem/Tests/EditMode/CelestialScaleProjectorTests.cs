using NUnit.Framework;
using Tanvir.SolarSystem.Mathematics;
using Tanvir.SolarSystem.Presentation.Scale;
using Tanvir.SolarSystem.Simulation;
using UnityEngine;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class CelestialScaleProjectorTests
    {
        private CelestialScaleProjector projector;

        [SetUp]
        public void SetUp()
        {
            projector = new CelestialScaleProjector(
                new PresentationScaleParameters(
                    1000000d,
                    160d,
                    6371d,
                    1d,
                    2.5d));
        }

        [Test]
        public void CoordinateAdapter_MapsCorePlaneAndNormalExactlyOnce()
        {
            Vector3 unity = UnityCoordinateAdapter.ToUnityVector(new Double3(1d, 2d, 3d));

            Assert.That(unity, Is.EqualTo(new Vector3(1f, 3f, 2f)));
        }

        [Test]
        public void RelativeDistanceProjection_IsZeroAtOriginAndMonotonic()
        {
            float zero = projector.ProjectRelativePosition(Double3.Zero).magnitude;
            float moon = projector.ProjectRelativePosition(new Double3(384400d, 0d, 0d)).magnitude;
            float earth = projector.ProjectRelativePosition(
                new Double3(149597870.7d, 0d, 0d)).magnitude;

            Assert.That(zero, Is.Zero);
            Assert.That(moon, Is.GreaterThan(zero));
            Assert.That(earth, Is.GreaterThan(moon));
        }

        [Test]
        public void RadiusProjection_UsesEarthReferenceWithoutExaggeration()
        {
            Assert.That(projector.ProjectRadius(6371d), Is.EqualTo(1f).Within(0.0001f));
            Assert.That(
                projector.ProjectRadius(1737.4d),
                Is.EqualTo(1737.4d / 6371d).Within(0.0001d));
            Assert.That(
                projector.ProjectRadius(69911d),
                Is.EqualTo(69911d / 6371d).Within(0.0001d));
            Assert.That(
                projector.ProjectRadius(695700d),
                Is.EqualTo(695700d / 6371d).Within(0.0001d));
        }

        [Test]
        public void PhysicalReference_PreparesExactGuidedComparisonRatios()
        {
            Assert.That(
                projector.PhysicalReference.ToDisplayUnits(6371d),
                Is.EqualTo(1d).Within(0.000000001d));
            Assert.That(
                projector.PhysicalReference.ToDisplayUnits(384400d),
                Is.EqualTo(384400d / 6371d).Within(0.000000001d));
            Assert.That(
                projector.PhysicalReference.ToDisplayUnits(149598261.1504425d),
                Is.EqualTo(149598261.1504425d / 6371d).Within(0.000000001d));
        }

        [Test]
        public void GasGiantProjection_PreservesScientificRadiusRatioAndDistanceOrdering()
        {
            float earthRadius = projector.ProjectRadius(6371d);
            float jupiterRadius = projector.ProjectRadius(69911d);
            float sunRadius = projector.ProjectRadius(695700d);
            float earthDistance = projector.ProjectRelativePosition(
                new Double3(149598261.150442527d, 0d, 0d)).magnitude;
            float jupiterPeriapsisDistance = projector.ProjectRelativePosition(
                new Double3(740678634.0040555d, 0d, 0d)).magnitude;
            float jupiterApoapsisDistance = projector.ProjectRelativePosition(
                new Double3(816002999.3813663d, 0d, 0d)).magnitude;

            Assert.That(jupiterRadius, Is.GreaterThan(earthRadius));
            Assert.That(jupiterRadius, Is.LessThan(sunRadius));
            Assert.That(
                jupiterRadius / earthRadius,
                Is.EqualTo(69911f / 6371f).Within(0.0001f));
            Assert.That(jupiterPeriapsisDistance, Is.GreaterThan(earthDistance + 100f));
            Assert.That(jupiterApoapsisDistance, Is.LessThan(480f));
        }

        [Test]
        public void CatalogProjection_ComposesMoonFromProjectedEarthPosition()
        {
            CelestialCatalog catalog = CelestialTestFactory.BuildCatalog(
                CelestialTestFactory.CreateSun(),
                CelestialTestFactory.CreateOrbitingBody(
                    "earth",
                    "sun",
                    semiMajorAxisKm: 1000000d),
                CelestialTestFactory.CreateOrbitingBody(
                    "moon",
                    "earth",
                    CelestialBodyCategory.Moon,
                    semiMajorAxisKm: 100000d));
            var evaluator = new KeplerOrbitEvaluator();
            var physical = new CelestialState[catalog.Count];
            var presentation = new CelestialPresentationState[catalog.Count];
            evaluator.Evaluate(catalog, 0d, physical);

            projector.Project(catalog, physical, presentation);

            Vector3 moonOffset = presentation[2].Position - presentation[1].Position;
            Assert.That(
                Vector3.Distance(
                    moonOffset,
                    projector.ProjectRelativePosition(physical[2].ParentRelativePositionKm)),
                Is.LessThan(0.00001f));
        }

        [Test]
        public void BufferEvaluator_MatchesReadOnlyConvenienceResult()
        {
            CelestialCatalog catalog = CelestialTestFactory.BuildCatalog(
                CelestialTestFactory.CreateSun(),
                CelestialTestFactory.CreateOrbitingBody("earth", "sun"));
            var evaluator = new KeplerOrbitEvaluator();
            var destination = new CelestialState[catalog.Count];

            evaluator.Evaluate(catalog, 12.5d, destination);
            var allocatingResult = evaluator.Evaluate(catalog, 12.5d);

            Assert.That(destination[0].PhysicalPositionKm, Is.EqualTo(allocatingResult[0].PhysicalPositionKm));
            Assert.That(destination[1].PhysicalPositionKm, Is.EqualTo(allocatingResult[1].PhysicalPositionKm));
            Assert.That(destination[1].RotationAngleDeg, Is.EqualTo(allocatingResult[1].RotationAngleDeg));
        }
    }
}
