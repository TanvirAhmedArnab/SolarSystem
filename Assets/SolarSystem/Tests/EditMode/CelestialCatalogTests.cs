using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Tanvir.SolarSystem.Simulation;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class CelestialCatalogTests
    {
        [Test]
        public void Build_ShuffledHierarchy_ReturnsDeterministicParentFirstOrder()
        {
            CelestialBodyModel sun = CelestialTestFactory.CreateSun();
            CelestialBodyModel earth = CelestialTestFactory.CreateOrbitingBody("earth", "sun");
            CelestialBodyModel mars = CelestialTestFactory.CreateOrbitingBody("mars", "sun");
            CelestialBodyModel moon = CelestialTestFactory.CreateOrbitingBody(
                "moon",
                "earth",
                CelestialBodyCategory.Moon);

            CelestialCatalog catalog = CelestialTestFactory.BuildCatalog(moon, mars, sun, earth);

            Assert.That(
                catalog.OrderedBodies.Select(body => body.Id.Value),
                Is.EqualTo(new[] { "sun", "earth", "mars", "moon" }));
        }

        [Test]
        public void Build_DuplicateIds_ThrowsDiagnostic()
        {
            CelestialBodyModel first = CelestialTestFactory.CreateOrbitingBody("earth", "sun");
            CelestialBodyModel duplicate = CelestialTestFactory.CreateOrbitingBody("earth", "sun");

            CelestialCatalogValidationException exception = Assert.Throws<CelestialCatalogValidationException>(
                () => CelestialTestFactory.BuildCatalog(CelestialTestFactory.CreateSun(), first, duplicate));

            Assert.That(exception.Message, Does.Contain("duplicated"));
        }

        [Test]
        public void Build_MissingParent_ThrowsDiagnostic()
        {
            CelestialBodyModel earth = CelestialTestFactory.CreateOrbitingBody("earth", "missing");

            CelestialCatalogValidationException exception = Assert.Throws<CelestialCatalogValidationException>(
                () => CelestialTestFactory.BuildCatalog(CelestialTestFactory.CreateSun(), earth));

            Assert.That(exception.Message, Does.Contain("missing parent"));
        }

        [Test]
        public void Build_ParentCycle_ThrowsDiagnostic()
        {
            CelestialBodyModel earth = CelestialTestFactory.CreateOrbitingBody("earth", "moon");
            CelestialBodyModel moon = CelestialTestFactory.CreateOrbitingBody(
                "moon",
                "earth",
                CelestialBodyCategory.Moon);

            CelestialCatalogValidationException exception = Assert.Throws<CelestialCatalogValidationException>(
                () => CelestialTestFactory.BuildCatalog(CelestialTestFactory.CreateSun(), earth, moon));

            Assert.That(exception.Message, Does.Contain("cycle"));
        }

        [TestCase(-0.01d)]
        [TestCase(1d)]
        [TestCase(double.NaN)]
        public void Build_InvalidEccentricity_ThrowsDiagnostic(double eccentricity)
        {
            CelestialBodyModel earth = CelestialTestFactory.CreateOrbitingBody(
                "earth",
                "sun",
                eccentricity: eccentricity);

            Assert.Throws<CelestialCatalogValidationException>(
                () => CelestialTestFactory.BuildCatalog(CelestialTestFactory.CreateSun(), earth));
        }

        [Test]
        public void OrderedBodies_AddAttempt_IsRejected()
        {
            CelestialCatalog catalog = CelestialTestFactory.BuildCatalog(CelestialTestFactory.CreateSun());
            var list = (IList<CelestialBodyModel>)catalog.OrderedBodies;

            Assert.Throws<NotSupportedException>(() => list.Add(CelestialTestFactory.CreateSun()));
        }

        [Test]
        public void GetBody_UnknownId_ThrowsKeyNotFoundException()
        {
            CelestialCatalog catalog = CelestialTestFactory.BuildCatalog(CelestialTestFactory.CreateSun());

            Assert.Throws<KeyNotFoundException>(() => catalog.GetBody(new CelestialBodyId("unknown")));
        }
    }
}
