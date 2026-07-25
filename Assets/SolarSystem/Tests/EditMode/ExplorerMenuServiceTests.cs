using NUnit.Framework;
using Tanvir.SolarSystem.Application;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class ExplorerMenuServiceTests
    {
        [Test]
        public void Menu_OpensSwitchesAndClosesWithOneChangePerStateTransition()
        {
            var menu = new ExplorerMenuService();
            int changes = 0;
            menu.Changed += () => changes++;

            menu.Open(ExplorerMenuPage.Help);
            menu.Open(ExplorerMenuPage.Help);
            menu.SetPage(ExplorerMenuPage.Settings);
            menu.SetPage(ExplorerMenuPage.CreditsAndSources);
            Assert.That(menu.Close(), Is.True);
            Assert.That(menu.Close(), Is.False);

            Assert.That(menu.IsOpen, Is.False);
            Assert.That(
                menu.ActivePage,
                Is.EqualTo(ExplorerMenuPage.CreditsAndSources));
            Assert.That(changes, Is.EqualTo(4));
        }
    }
}
