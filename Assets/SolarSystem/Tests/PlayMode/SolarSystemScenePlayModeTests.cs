using System.Collections;
using NUnit.Framework;
using Tanvir.SolarSystem.Application;
using Tanvir.SolarSystem.Interaction;
using Tanvir.SolarSystem.Presentation.Camera;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
using Tanvir.SolarSystem.Presentation.UI;
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

        [UnityTest]
        public IEnumerator SolarSystemScene_SelectsFocusesRedirectsAndReturnsToFreeFlight()
        {
            SceneManager.LoadScene("SolarSystem", LoadSceneMode.Single);
            yield return null;

            SolarSystemCompositionRoot simulation =
                Object.FindAnyObjectByType<SolarSystemCompositionRoot>();
            SolarSystemInteractionCompositionRoot interaction =
                Object.FindAnyObjectByType<SolarSystemInteractionCompositionRoot>();
            Assert.That(simulation, Is.Not.Null);
            Assert.That(interaction, Is.Not.Null);
            Assert.That(interaction.IsInitialized, Is.True);

            simulation.SimulationController.SetPaused(true);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "earth",
                    out CelestialBodyView earth),
                Is.True);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "jupiter",
                    out CelestialBodyView jupiter),
                Is.True);

            Camera camera = Camera.main;
            CelestialSelectionController selection = interaction.SelectionController;
            SolarSystemCameraController cameraController = interaction.CameraController;
            Physics.SyncTransforms();
            Vector3 earthScreen = camera.WorldToScreenPoint(earth.transform.position);

            Assert.That(
                selection.SelectAtScreenPosition(earthScreen),
                Is.True,
                "The Earth selection collider should resolve from the camera ray.");
            Assert.That(selection.SelectedView, Is.SameAs(earth));
            Assert.That(selection.Service.SelectedId.Value.Value, Is.EqualTo("earth"));

            cameraController.Focus(earth);
            yield return new WaitForSecondsRealtime(0.75f);
            Assert.That(cameraController.Mode, Is.EqualTo(SolarSystemCameraMode.Focused));
            Assert.That(cameraController.FocusedTarget, Is.SameAs(earth));
            AssertCameraFaces(camera, earth);

            cameraController.Focus(jupiter);
            yield return new WaitForSecondsRealtime(0.75f);
            Assert.That(cameraController.Mode, Is.EqualTo(SolarSystemCameraMode.Focused));
            Assert.That(cameraController.FocusedTarget, Is.SameAs(jupiter));
            AssertCameraFaces(camera, jupiter);

            cameraController.ReturnToFreeFlight();
            Vector3 beforeFlight = camera.transform.position;
            cameraController.StepFreeFlight(
                Vector2.up,
                0f,
                Vector2.zero,
                false,
                0.25f);

            Assert.That(cameraController.Mode, Is.EqualTo(SolarSystemCameraMode.FreeFlight));
            Assert.That(cameraController.FocusedTarget, Is.Null);
            Assert.That(
                Vector3.Distance(beforeFlight, camera.transform.position),
                Is.GreaterThan(0.01f));
        }

        [UnityTest]
        public IEnumerator SolarSystemScene_TimeCommandsUpdateMotionAndHudFeedback()
        {
            SceneManager.LoadScene("SolarSystem", LoadSceneMode.Single);
            yield return null;

            SolarSystemCompositionRoot simulation =
                Object.FindAnyObjectByType<SolarSystemCompositionRoot>();
            SolarSystemInteractionCompositionRoot interaction =
                Object.FindAnyObjectByType<SolarSystemInteractionCompositionRoot>();
            Assert.That(simulation, Is.Not.Null);
            Assert.That(interaction, Is.Not.Null);
            Assert.That(interaction.IsInitialized, Is.True);

            SimulationTimeControlService timeControls = interaction.TimeControls;
            SolarSystemHudPresenter hud = interaction.HudPresenter;
            Assert.That(timeControls, Is.Not.Null);
            Assert.That(hud, Is.Not.Null);
            Assert.That(hud.IsInitialized, Is.True);
            Assert.That(timeControls.CurrentMultiplier, Is.EqualTo(10));
            Assert.That(hud.SimulationStateText, Does.Contain("RUNNING"));
            Assert.That(hud.SimulationRateText, Does.Contain("10x"));
            Assert.That(hud.PauseActionText, Is.EqualTo("PAUSE"));

            Assert.That(
                simulation.SimulationController.TryGetView(
                    "earth",
                    out CelestialBodyView earth),
                Is.True);
            interaction.SelectionController.Select(earth);
            Assert.That(hud.SelectionText, Does.Contain("EARTH"));
            yield return null;
            Assert.That(hud.IsBodyInformationVisible, Is.True);
            Assert.That(hud.IsSelectionReticleVisible, Is.True);
            Assert.That(hud.BodyNameText, Is.EqualTo("Earth"));
            Assert.That(hud.BodyRadiusText, Is.EqualTo("6,371.0 km"));
            Assert.That(
                hud.BodySourceText,
                Is.EqualTo("NASA_NSSDC_EARTH_AND_JPL_APPROX_POS_J2000"));

            timeControls.TogglePaused();
            Vector3 pausedPosition = earth.transform.position;
            yield return new WaitForSecondsRealtime(0.1f);
            Assert.That(timeControls.IsPaused, Is.True);
            Assert.That(hud.SimulationStateText, Does.Contain("PAUSED"));
            Assert.That(hud.PauseActionText, Is.EqualTo("RESUME"));
            Assert.That(
                Vector3.Distance(pausedPosition, earth.transform.position),
                Is.LessThan(0.00001f));

            Assert.That(timeControls.IncreaseSpeed(), Is.True);
            Assert.That(timeControls.CurrentMultiplier, Is.EqualTo(100));
            Assert.That(hud.SimulationRateText, Does.Contain("100x"));

            timeControls.TogglePaused();
            Vector3 resumedPosition = earth.transform.position;
            yield return new WaitForSecondsRealtime(0.1f);
            Assert.That(timeControls.IsPaused, Is.False);
            Assert.That(hud.PauseActionText, Is.EqualTo("PAUSE"));
            Assert.That(
                Vector3.Distance(resumedPosition, earth.transform.position),
                Is.GreaterThan(0.001f));
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

        private static void AssertCameraFaces(Camera camera, CelestialBodyView view)
        {
            Vector3 direction =
                (view.transform.position - camera.transform.position).normalized;
            Assert.That(Vector3.Dot(camera.transform.forward, direction), Is.GreaterThan(0.999f));
            Assert.That(
                Vector3.Distance(camera.transform.position, view.transform.position),
                Is.GreaterThan(view.CurrentDisplayRadius));
        }
    }
}
