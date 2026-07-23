using NUnit.Framework;
using Tanvir.SolarSystem.Interaction;
using Tanvir.SolarSystem.Simulation;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class SelectionServiceTests
    {
        [Test]
        public void Select_StoresStableIdAndRaisesOneChange()
        {
            var service = new SelectionService();
            var earth = new CelestialBodyId("earth");
            int changeCount = 0;
            service.SelectionChanged += _ => changeCount++;

            service.Select(earth);
            service.Select(earth);

            Assert.That(service.SelectedId, Is.EqualTo(earth));
            Assert.That(changeCount, Is.EqualTo(1));
        }

        [Test]
        public void Select_DifferentBody_ReplacesSelection()
        {
            var service = new SelectionService();
            var earth = new CelestialBodyId("earth");
            var jupiter = new CelestialBodyId("jupiter");

            service.Select(earth);
            service.Select(jupiter);

            Assert.That(service.SelectedId, Is.EqualTo(jupiter));
        }

        [Test]
        public void Clear_OnlyRaisesWhenSelectionExists()
        {
            var service = new SelectionService();
            int changeCount = 0;
            service.SelectionChanged += _ => changeCount++;

            service.Clear();
            service.Select(new CelestialBodyId("earth"));
            service.Clear();
            service.Clear();

            Assert.That(service.SelectedId, Is.Null);
            Assert.That(changeCount, Is.EqualTo(2));
        }
    }
}
