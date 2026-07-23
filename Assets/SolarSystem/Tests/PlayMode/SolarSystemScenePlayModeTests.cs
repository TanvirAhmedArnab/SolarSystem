using System.Collections;
using NUnit.Framework;
using Tanvir.SolarSystem.Application;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tanvir.SolarSystem.Tests.PlayMode
{
    public sealed class SolarSystemScenePlayModeTests
    {
        [UnityTest]
        public IEnumerator SolarSystemScene_BootstrapsMovesAndPausesAllBodies()
        {
            SceneManager.LoadScene("SolarSystem", LoadSceneMode.Single);
            yield return null;

            SolarSystemCompositionRoot composition =
                Object.FindAnyObjectByType<SolarSystemCompositionRoot>();
            Assert.That(composition, Is.Not.Null);
            Assert.That(composition.IsInitialized, Is.True);
            Assert.That(composition.SimulationController.CatalogCount, Is.EqualTo(3));
            Assert.That(composition.SimulationController.ViewCount, Is.EqualTo(3));

            Assert.That(
                composition.SimulationController.TryGetView("earth", out CelestialBodyView earth),
                Is.True);
            Assert.That(
                composition.SimulationController.TryGetView("moon", out CelestialBodyView moon),
                Is.True);

            Vector3 earthBefore = earth.transform.position;
            Vector3 moonBefore = moon.transform.position;
            yield return new WaitForSecondsRealtime(0.1f);

            Assert.That(Vector3.Distance(earthBefore, earth.transform.position), Is.GreaterThan(0.001f));
            Assert.That(Vector3.Distance(moonBefore, moon.transform.position), Is.GreaterThan(0.001f));

            composition.SimulationController.SetPaused(true);
            yield return null;
            Vector3 earthPaused = earth.transform.position;
            Vector3 moonPaused = moon.transform.position;
            yield return new WaitForSecondsRealtime(0.1f);

            Assert.That(Vector3.Distance(earthPaused, earth.transform.position), Is.LessThan(0.00001f));
            Assert.That(Vector3.Distance(moonPaused, moon.transform.position), Is.LessThan(0.00001f));
            Assert.That(
                Object.FindObjectsByType<CelestialOrbitPathView>().Length,
                Is.EqualTo(2));
        }
    }
}
