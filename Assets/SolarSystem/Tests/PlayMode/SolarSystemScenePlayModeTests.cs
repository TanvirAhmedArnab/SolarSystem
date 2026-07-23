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
            Assert.That(composition.SimulationController.CatalogCount, Is.EqualTo(4));
            Assert.That(composition.SimulationController.ViewCount, Is.EqualTo(4));

            Assert.That(
                composition.SimulationController.TryGetView("sun", out CelestialBodyView sun),
                Is.True);
            Assert.That(
                composition.SimulationController.TryGetView("earth", out CelestialBodyView earth),
                Is.True);
            Assert.That(
                composition.SimulationController.TryGetView("moon", out CelestialBodyView moon),
                Is.True);
            Assert.That(
                composition.SimulationController.TryGetView(
                    "jupiter",
                    out CelestialBodyView jupiter),
                Is.True);

            Vector3 earthBefore = earth.transform.position;
            Vector3 moonBefore = moon.transform.position;
            Vector3 jupiterBefore = jupiter.transform.position;
            yield return new WaitForSecondsRealtime(0.1f);

            Assert.That(Vector3.Distance(earthBefore, earth.transform.position), Is.GreaterThan(0.001f));
            Assert.That(Vector3.Distance(moonBefore, moon.transform.position), Is.GreaterThan(0.001f));
            Assert.That(
                Vector3.Distance(jupiterBefore, jupiter.transform.position),
                Is.GreaterThan(0.0001f));

            composition.SimulationController.SetPaused(true);
            yield return null;
            Vector3 earthPaused = earth.transform.position;
            Vector3 moonPaused = moon.transform.position;
            Vector3 jupiterPaused = jupiter.transform.position;
            yield return new WaitForSecondsRealtime(0.1f);

            Assert.That(Vector3.Distance(earthPaused, earth.transform.position), Is.LessThan(0.00001f));
            Assert.That(Vector3.Distance(moonPaused, moon.transform.position), Is.LessThan(0.00001f));
            Assert.That(
                Vector3.Distance(jupiterPaused, jupiter.transform.position),
                Is.LessThan(0.00001f));
            Assert.That(
                Object.FindObjectsByType<CelestialOrbitPathView>().Length,
                Is.EqualTo(3));

            Camera camera = Camera.main;
            Assert.That(camera, Is.Not.Null);
            AssertWithinViewport(camera, sun);
            AssertWithinViewport(camera, earth);
            AssertWithinViewport(camera, moon);
            AssertWithinViewport(camera, jupiter);
        }

        private static void AssertWithinViewport(Camera camera, CelestialBodyView view)
        {
            Vector3 viewport = camera.WorldToViewportPoint(view.transform.position);
            Assert.That(viewport.z, Is.GreaterThan(0f), $"{view.name} is behind the camera.");
            Assert.That(
                viewport.x,
                Is.InRange(0f, 1f),
                $"{view.name} is outside horizontal framing.");
            Assert.That(
                viewport.y,
                Is.InRange(0f, 1f),
                $"{view.name} is outside vertical framing.");
        }
    }
}
