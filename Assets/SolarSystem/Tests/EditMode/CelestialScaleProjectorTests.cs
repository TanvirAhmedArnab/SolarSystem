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
                    15d,
                    6371d,
                    0.8d,
                    0.4d,
                    0.18d,
                    4.8d));
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
        public void RadiusProjection_UsesReferenceAndExplicitClamps()
        {
            Assert.That(projector.ProjectRadius(6371d), Is.EqualTo(0.8f).Within(0.0001f));
            Assert.That(projector.ProjectRadius(1d), Is.EqualTo(0.18f).Within(0.0001f));
            Assert.That(projector.ProjectRadius(695700d), Is.EqualTo(4.8f).Within(0.0001f));
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
