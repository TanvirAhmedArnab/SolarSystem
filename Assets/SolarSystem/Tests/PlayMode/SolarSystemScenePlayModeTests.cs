using System.Collections;
using NUnit.Framework;
using Tanvir.SolarSystem.Application;
using Tanvir.SolarSystem.Interaction;
using Tanvir.SolarSystem.Presentation.Camera;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
using Tanvir.SolarSystem.Presentation.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tanvir.SolarSystem.Tests.PlayMode
{
    public sealed class SolarSystemScenePlayModeTests
    {
        private const float FocusTransitionTimeoutSeconds = 2f;
        private static readonly string[] ExpectedBodyIds =
        {
            "sun",
            "mercury",
            "venus",
            "earth",
            "moon",
            "mars",
            "jupiter",
            "saturn",
            "uranus",
            "neptune"
        };

        [UnityTest]
        public IEnumerator SolarSystemScene_BootstrapsMovesAndPausesAllBodies()
        {
            SceneManager.LoadScene("SolarSystem", LoadSceneMode.Single);
            yield return null;

            SolarSystemCompositionRoot composition =
                Object.FindAnyObjectByType<SolarSystemCompositionRoot>();
            Assert.That(composition, Is.Not.Null);
            Assert.That(composition.IsInitialized, Is.True);
            Assert.That(composition.SimulationController.CatalogCount, Is.EqualTo(10));
            Assert.That(composition.SimulationController.ViewCount, Is.EqualTo(10));
            foreach (string stableId in ExpectedBodyIds)
            {
                Assert.That(
                    composition.SimulationController.TryGetView(
                        stableId,
                        out CelestialBodyView _),
                    Is.True,
                    $"The scene should contain the authored '{stableId}' view.");
            }

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
                Is.EqualTo(9));

            Camera camera = Camera.main;
            Assert.That(camera, Is.Not.Null);
            foreach (string stableId in ExpectedBodyIds)
            {
                Assert.That(
                    composition.SimulationController.TryGetView(
                        stableId,
                        out CelestialBodyView framedView),
                    Is.True);
                AssertWithinViewport(camera, framedView);
            }
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
            yield return WaitUntilFocused(cameraController);
            Assert.That(cameraController.FocusedTarget, Is.SameAs(earth));
            AssertCameraFaces(camera, earth);

            cameraController.Focus(jupiter);
            yield return WaitUntilFocused(cameraController);
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

        [UnityTest]
        public IEnumerator SolarSystemScene_UsesProjectOwnedVisualFoundation()
        {
            SceneManager.LoadScene("SolarSystem", LoadSceneMode.Single);
            yield return null;

            Camera camera = Camera.main;
            Assert.That(camera, Is.Not.Null);
            Assert.That(camera.clearFlags, Is.EqualTo(CameraClearFlags.Skybox));
            Assert.That(camera.allowHDR, Is.True);

            UniversalAdditionalCameraData cameraData =
                camera.GetUniversalAdditionalCameraData();
            Assert.That(cameraData.renderPostProcessing, Is.True);
            Assert.That(cameraData.stopNaN, Is.True);
            Assert.That(cameraData.dithering, Is.True);

            Volume volume = Object.FindAnyObjectByType<Volume>();
            Assert.That(volume, Is.Not.Null);
            Assert.That(volume.isGlobal, Is.True);
            Assert.That(volume.sharedProfile, Is.Not.Null);
            Assert.That(volume.sharedProfile.name, Is.EqualTo("VP_SolarSystem"));

            Assert.That(RenderSettings.skybox, Is.Not.Null);
            Assert.That(RenderSettings.skybox.name, Is.EqualTo("M_SpaceSkybox"));
            Assert.That(RenderSettings.ambientMode, Is.EqualTo(AmbientMode.Flat));
            Assert.That(RenderSettings.reflectionIntensity, Is.EqualTo(0.18f).Within(0.001f));

            Assert.That(RenderSettings.sun, Is.Null);
        }

        [UnityTest]
        public IEnumerator SolarSystemScene_UsesSunOriginRadialIllumination()
        {
            SceneManager.LoadScene("SolarSystem", LoadSceneMode.Single);
            yield return null;

            SolarSystemCompositionRoot simulation =
                Object.FindAnyObjectByType<SolarSystemCompositionRoot>();
            Assert.That(simulation, Is.Not.Null);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "sun",
                    out CelestialBodyView sun),
                Is.True);
            GameObject radialLightObject = GameObject.Find("Solar Radial Light");
            Assert.That(radialLightObject, Is.Not.Null);
            Light radialLight = radialLightObject.GetComponent<Light>();
            Assert.That(radialLight, Is.Not.Null);
            Assert.That(radialLight.name, Is.EqualTo("Solar Radial Light"));
            Assert.That(radialLight.type, Is.EqualTo(LightType.Point));
            Assert.That(radialLight.lightUnit, Is.EqualTo(LightUnit.Candela));
            Assert.That(radialLight.intensity, Is.EqualTo(1450f).Within(0.001f));
            Assert.That(radialLight.range, Is.EqualTo(80f).Within(0.001f));
            Assert.That(radialLight.shadows, Is.EqualTo(LightShadows.None));
            Assert.That(radialLight.transform.parent, Is.SameAs(sun.transform));
            Assert.That(
                Vector3.Distance(radialLight.transform.position, sun.transform.position),
                Is.LessThan(0.00001f));

            foreach (string stableId in ExpectedBodyIds)
            {
                if (stableId == "sun")
                {
                    continue;
                }

                Assert.That(
                    simulation.SimulationController.TryGetView(
                        stableId,
                        out CelestialBodyView receiver),
                    Is.True);
                AssertReceivesSunOriginLight(radialLight, sun, receiver);
            }

            Transform saturnRings = GameObject.Find("Saturn")?.transform.Find("Visual/Rings");
            Assert.That(saturnRings, Is.Not.Null);
            Assert.That(saturnRings.GetComponent<MeshFilter>().sharedMesh.name, Is.EqualTo("SM_Saturn_Rings"));
            Assert.That(
                saturnRings.GetComponent<MeshRenderer>().sharedMaterial.name,
                Is.EqualTo("M_Saturn_Rings"));

            yield return new WaitForSecondsRealtime(0.1f);
            Assert.That(
                Vector3.Distance(radialLight.transform.position, sun.transform.position),
                Is.LessThan(0.00001f));
            Assert.That(RenderSettings.sun, Is.Null);
        }

        private static void AssertReceivesSunOriginLight(
            Light radialLight,
            CelestialBodyView sun,
            CelestialBodyView receiver)
        {
            Vector3 receiverToLight =
                (radialLight.transform.position - receiver.transform.position).normalized;
            Vector3 receiverToSun =
                (sun.transform.position - receiver.transform.position).normalized;
            Assert.That(
                Vector3.Dot(receiverToLight, receiverToSun),
                Is.GreaterThan(0.99999f),
                $"{receiver.name} must receive light from the live Sun direction.");
            Assert.That(
                Vector3.Distance(
                    radialLight.transform.position,
                    receiver.transform.position) + receiver.CurrentDisplayRadius,
                Is.LessThan(radialLight.range),
                $"{receiver.name}'s complete visible sphere must remain inside the light range.");
        }

        private static IEnumerator WaitUntilFocused(
            SolarSystemCameraController cameraController)
        {
            float deadline = Time.realtimeSinceStartup + FocusTransitionTimeoutSeconds;
            while (cameraController.Mode == SolarSystemCameraMode.FocusTransition &&
                   Time.realtimeSinceStartup < deadline)
            {
                yield return null;
            }

            Assert.That(
                cameraController.Mode,
                Is.EqualTo(SolarSystemCameraMode.Focused),
                $"Camera did not finish focusing within {FocusTransitionTimeoutSeconds:F1} seconds.");
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
