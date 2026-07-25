using NUnit.Framework;
using Tanvir.SolarSystem.Application;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class CelestialNavigationServiceTests
    {
        private CelestialNavigationService service;

        [SetUp]
        public void SetUp()
        {
            service = new CelestialNavigationService();
        }

        [Test]
        public void Defaults_KeepNavigatorClosedAndLabelsEnabled()
        {
            Assert.That(service.IsNavigatorVisible, Is.False);
            Assert.That(service.AreWorldLabelsEnabled, Is.True);
        }

        [Test]
        public void NavigatorCommands_ReportOnlyEffectiveChanges()
        {
            int changes = 0;
            service.Changed += () => changes++;

            service.SetNavigatorVisible(false);
            service.ToggleNavigator();
            service.SetNavigatorVisible(true);
            service.ToggleNavigator();

            Assert.That(service.IsNavigatorVisible, Is.False);
            Assert.That(changes, Is.EqualTo(2));
        }

        [Test]
        public void LabelCommands_ReportOnlyEffectiveChanges()
        {
            int changes = 0;
            service.Changed += () => changes++;

            service.SetWorldLabelsEnabled(true);
            service.ToggleWorldLabels();
            service.SetWorldLabelsEnabled(false);
            service.ToggleWorldLabels();

            Assert.That(service.AreWorldLabelsEnabled, Is.True);
            Assert.That(changes, Is.EqualTo(2));
        }
    }
}
