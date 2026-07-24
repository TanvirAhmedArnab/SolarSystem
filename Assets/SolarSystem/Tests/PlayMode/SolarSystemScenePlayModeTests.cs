using System.Collections;
using NUnit.Framework;
using Tanvir.SolarSystem.Application;
using Tanvir.SolarSystem.Audio;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Interaction;
using Tanvir.SolarSystem.Mathematics;
using Tanvir.SolarSystem.Presentation.Camera;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
using Tanvir.SolarSystem.Presentation.Lighting;
using Tanvir.SolarSystem.Presentation.Scale;
using Tanvir.SolarSystem.Presentation.UI;
using Tanvir.SolarSystem.Simulation;
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
            CelestialOrbitPathVisibilityController orbitVisibility =
                Object.FindAnyObjectByType<CelestialOrbitPathVisibilityController>();
            Assert.That(orbitVisibility, Is.Not.Null);
            Assert.That(orbitVisibility.IsInitialized, Is.True);
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
            yield return null;
            Assert.That(cameraController.FocusedTarget, Is.SameAs(earth));
            AssertCameraFaces(camera, earth);
            Assert.That(orbitVisibility.ArePathsVisible, Is.False);
            Assert.That(
                Object.FindObjectsByType<CelestialOrbitPathView>(FindObjectsSortMode.None),
                Has.All.Matches<CelestialOrbitPathView>(
                    path => !path.GetComponent<LineRenderer>().enabled));

            cameraController.Focus(jupiter);
            yield return WaitUntilFocused(cameraController);
            Assert.That(cameraController.FocusedTarget, Is.SameAs(jupiter));
            AssertCameraFaces(camera, jupiter);

            cameraController.ReturnToFreeFlight();
            yield return null;
            Vector3 beforeFlight = camera.transform.position;
            cameraController.StepFreeFlight(
                Vector2.up,
                0f,
                Vector2.zero,
                false,
                0.25f);

            Assert.That(cameraController.Mode, Is.EqualTo(SolarSystemCameraMode.FreeFlight));
            Assert.That(cameraController.FocusedTarget, Is.Null);
            Assert.That(orbitVisibility.ArePathsVisible, Is.True);
            Assert.That(
                Object.FindObjectsByType<CelestialOrbitPathView>(FindObjectsSortMode.None),
                Has.All.Matches<CelestialOrbitPathView>(
                    path => path.GetComponent<LineRenderer>().enabled));
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
            Assert.That(timeControls.CurrentMultiplier, Is.EqualTo(1));
            Assert.That(hud.SimulationStateText, Does.Contain("RUNNING"));
            Assert.That(hud.SimulationRateText, Does.Contain("1x"));
            Assert.That(hud.SimulationRateText, Does.Contain("EARTH ROTATION"));
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
            Assert.That(
                hud.BodyScaleNoteText,
                Does.Contain("ATMOSPHERE THICKNESS"));

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
            Assert.That(timeControls.CurrentMultiplier, Is.EqualTo(10));
            Assert.That(hud.SimulationRateText, Does.Contain("10x"));

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
        public IEnumerator SolarSystemScene_UsesLicensedEventDrivenAudioBaseline()
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

            AudioDirector audio = interaction.AudioDirector;
            Assert.That(audio, Is.Not.Null);
            Assert.That(audio.IsInitialized, Is.True);
            Assert.That(audio.MusicSource.clip.name, Is.EqualTo("A_Music_OuterSpaceLoop"));
            Assert.That(audio.MusicSource.loop, Is.True);
            Assert.That(audio.MusicSource.playOnAwake, Is.True);
            Assert.That(audio.MusicSource.spatialBlend, Is.Zero);

            Assert.That(audio.SunAmbienceSource.clip.name, Is.EqualTo("A_Sun_BurningLoop"));
            Assert.That(audio.SunAmbienceSource.loop, Is.True);
            Assert.That(audio.SunAmbienceSource.spatialBlend, Is.Zero);

            Assert.That(
                simulation.SimulationController.TryGetView(
                    "earth",
                    out CelestialBodyView earth),
                Is.True);
            Assert.That(
                audio.EarthAmbienceSource.clip.name,
                Is.EqualTo("A_Earth_ForestAmbienceLoop"));
            Assert.That(audio.EarthAmbienceSource.transform.parent, Is.SameAs(earth.transform));
            Assert.That(audio.EarthAmbienceSource.loop, Is.True);
            Assert.That(audio.EarthAmbienceSource.spatialBlend, Is.EqualTo(1f));
            Assert.That(
                audio.EarthAmbienceSource.rolloffMode,
                Is.EqualTo(AudioRolloffMode.Logarithmic));
            Assert.That(audio.EarthAmbienceSource.minDistance, Is.EqualTo(1.5f));
            Assert.That(audio.EarthAmbienceSource.maxDistance, Is.EqualTo(12f));
            Assert.That(audio.EarthAmbienceSource.dopplerLevel, Is.Zero);
            Assert.That(audio.UiSource.spatialBlend, Is.Zero);
            Assert.That(audio.UiSource.playOnAwake, Is.False);

            interaction.SelectionController.Select(earth);
            Assert.That(audio.LastFeedbackCue, Is.EqualTo(AudioFeedbackCue.Selection));
            Assert.That(audio.FeedbackCueCount, Is.EqualTo(1));

            interaction.CameraController.Focus(earth);
            Assert.That(audio.LastFeedbackCue, Is.EqualTo(AudioFeedbackCue.Focus));
            Assert.That(audio.FeedbackCueCount, Is.EqualTo(2));

            interaction.TimeControls.TogglePaused();
            Assert.That(audio.LastFeedbackCue, Is.EqualTo(AudioFeedbackCue.TimeControl));
            Assert.That(audio.FeedbackCueCount, Is.EqualTo(3));

            audio.SetMuted(true);
            Assert.That(audio.IsMuted, Is.True);
            Assert.That(audio.MusicSource.mute, Is.True);
            Assert.That(audio.SunAmbienceSource.mute, Is.True);
            Assert.That(audio.EarthAmbienceSource.mute, Is.True);
            Assert.That(audio.UiSource.mute, Is.True);
        }

        [UnityTest]
        public IEnumerator SolarSystemScene_UsesEarthReferencedSizesAndSignedSiderealSpin()
        {
            SceneManager.LoadScene("SolarSystem", LoadSceneMode.Single);
            yield return null;

            SolarSystemCompositionRoot composition =
                Object.FindAnyObjectByType<SolarSystemCompositionRoot>();
            Assert.That(composition, Is.Not.Null);
            Assert.That(composition.IsInitialized, Is.True);
            Assert.That(
                composition.SimulationController.ClockSnapshot.SpeedMultiplier,
                Is.EqualTo(CelestialReferenceUnits.EarthSiderealRotationPeriodSeconds)
                    .Within(0.000001d));

            double simulationTime = composition.SimulationController
                .ClockSnapshot.ElapsedSimulationTimeSeconds;
            var evaluator = new KeplerOrbitEvaluator();
            foreach (string stableId in ExpectedBodyIds)
            {
                Assert.That(
                    composition.SimulationController.TryGetView(
                        stableId,
                        out CelestialBodyView view),
                    Is.True);
                CelestialBodyDefinition definition = view.Definition;
                Assert.That(definition, Is.Not.Null);
                double expectedRadius =
                    definition.MeanRadiusKm / CelestialReferenceUnits.EarthMeanRadiusKm;
                Assert.That(
                    view.CurrentDisplayRadius,
                    Is.EqualTo(expectedRadius).Within(0.0001d),
                    $"{definition.DisplayName} must retain its Earth-relative mean-radius ratio.");
                SphereCollider selectionCollider = view.GetComponent<SphereCollider>();
                Assert.That(selectionCollider, Is.Not.Null);
                Assert.That(
                    selectionCollider.radius,
                    Is.EqualTo(
                        Mathf.Max(
                            view.CurrentDisplayRadius,
                            ReadableOverviewScaleContract.MinimumSelectionRadius))
                        .Within(0.0001f),
                    $"{definition.DisplayName}'s accessible hit area must not change its visual size.");

                CelestialBodyModel model = definition.ToModel();
                CelestialState state =
                    evaluator.Evaluate(model, Double3.Zero, simulationTime);
                Quaternion expectedRotation =
                    Quaternion.AngleAxis((float)model.AxialTiltDeg, Vector3.forward) *
                    Quaternion.AngleAxis(-(float)state.RotationAngleDeg, Vector3.up);
                Transform visual = view.transform.Find("Visual");
                Assert.That(visual, Is.Not.Null);
                Assert.That(
                    Quaternion.Angle(visual.localRotation, expectedRotation),
                    Is.LessThan(0.001f),
                    $"{definition.DisplayName} must apply its signed sidereal spin direction.");
            }

            Assert.That(
                composition.SimulationController.TryGetView(
                    "earth",
                    out CelestialBodyView earth),
                Is.True);
            Assert.That(earth.CurrentDisplayRadius, Is.EqualTo(1f).Within(0.0001f));
        }

        [UnityTest]
        public IEnumerator SolarSystemScene_GuidesScaleComparisonAndRestoresExplorerState()
        {
            SceneManager.LoadScene("SolarSystem", LoadSceneMode.Single);
            yield return null;

            SolarSystemCompositionRoot composition =
                Object.FindAnyObjectByType<SolarSystemCompositionRoot>();
            SolarSystemInteractionCompositionRoot interaction =
                Object.FindAnyObjectByType<SolarSystemInteractionCompositionRoot>();
            Assert.That(composition, Is.Not.Null);
            Assert.That(interaction, Is.Not.Null);
            Assert.That(interaction.IsInitialized, Is.True);

            SolarSystemSimulationController simulation =
                composition.SimulationController;
            GuidedScaleComparisonService comparison = interaction.ScaleComparison;
            SolarSystemCameraController cameraController =
                interaction.CameraController;
            SolarSystemHudPresenter hud = interaction.HudPresenter;
            Camera camera = Camera.main;
            Assert.That(comparison, Is.Not.Null);
            Assert.That(camera, Is.Not.Null);
            Assert.That(
                simulation.TryGetView("earth", out CelestialBodyView earth),
                Is.True);
            Assert.That(
                simulation.TryGetView("sun", out CelestialBodyView sun),
                Is.True);

            interaction.SelectionController.Select(earth);
            Vector3 savedPosition = camera.transform.position;
            Quaternion savedRotation = camera.transform.rotation;
            float savedNear = camera.nearClipPlane;
            float savedFar = camera.farClipPlane;

            comparison.Advance();
            yield return WaitUntilGuided(cameraController);
            Assert.That(comparison.Stage, Is.EqualTo(
                GuidedScaleComparisonStage.ReadableOverview));
            Assert.That(interaction.TimeControls.IsPaused, Is.True);
            Assert.That(hud.IsScaleComparisonVisible, Is.True);
            Assert.That(hud.ScaleComparisonTitleText, Is.EqualTo("READABLE OVERVIEW"));
            Assert.That(hud.ScaleModeText, Does.Contain("ORBITS COMPRESSED"));
            Assert.That(hud.IsBodyInformationVisible, Is.False);
            Assert.That(
                interaction.SelectionController.SelectedView,
                Is.SameAs(earth));

            comparison.Advance();
            yield return WaitUntilGuided(cameraController);
            Assert.That(simulation.ScaleMode, Is.EqualTo(
                CelestialScaleMode.NormalizedOrbits));
            Assert.That(hud.ScaleComparisonTitleText, Is.EqualTo(
                "LINEAR ORBIT SPACING"));
            Assert.That(hud.ScaleComparisonMetricText, Does.Contain(
                "37.659 MILLION KM"));
            Assert.That(
                earth.CurrentDisplayRadius,
                Is.EqualTo(
                    CelestialReferenceUnits.EarthMeanRadiusKm /
                    GuidedScaleComparisonContract.MercuryVenusEnvelopeGapKm)
                    .Within(0.000000001d));
            foreach (CelestialOrbitPathView path in
                     Object.FindObjectsByType<CelestialOrbitPathView>())
            {
                Assert.That(
                    path.GetComponent<LineRenderer>().widthMultiplier,
                    Is.EqualTo(
                        GuidedScaleComparisonContract.NormalizedOrbitLineWidth)
                        .Within(0.0001f));
            }

            comparison.Advance();
            yield return WaitUntilGuided(cameraController);
            Assert.That(simulation.ScaleMode, Is.EqualTo(
                CelestialScaleMode.LiteralEarthReference));
            Assert.That(hud.ScaleComparisonTitleText, Is.EqualTo(
                "LITERAL EARTH-RADIUS REFERENCE"));
            Assert.That(hud.ScaleComparisonMetricText, Does.Contain("23,481"));
            Assert.That(earth.transform.position, Is.EqualTo(Vector3.zero));
            Assert.That(earth.CurrentDisplayRadius, Is.EqualTo(1f).Within(0.0001f));
            Assert.That(sun.transform.position.magnitude, Is.GreaterThan(23000f));
            AssertWithinViewport(camera, earth);
            AssertWithinViewport(camera, sun);

            Assert.That(comparison.Cancel(), Is.True);
            yield return WaitUntilExplorerRestored(cameraController);
            yield return null;

            Assert.That(comparison.IsActive, Is.False);
            Assert.That(simulation.ScaleMode, Is.EqualTo(
                CelestialScaleMode.ReadableOverview));
            Assert.That(interaction.TimeControls.IsPaused, Is.False);
            Assert.That(hud.IsScaleComparisonVisible, Is.False);
            Assert.That(hud.IsBodyInformationVisible, Is.True);
            Assert.That(
                interaction.SelectionController.SelectedView,
                Is.SameAs(earth));
            Assert.That(
                Vector3.Distance(camera.transform.position, savedPosition),
                Is.LessThan(0.001f));
            Assert.That(
                Quaternion.Angle(camera.transform.rotation, savedRotation),
                Is.LessThan(0.001f));
            Assert.That(camera.nearClipPlane, Is.EqualTo(savedNear).Within(0.001f));
            Assert.That(camera.farClipPlane, Is.EqualTo(savedFar).Within(0.001f));
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
            Assert.That(radialLight.intensity, Is.EqualTo(165000f).Within(0.001f));
            Assert.That(radialLight.range, Is.EqualTo(620f).Within(0.001f));
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

        [UnityTest]
        public IEnumerator SolarSystemScene_UsesLayeredEarthRenderingAndDeterministicCloudMotion()
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
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "earth",
                    out CelestialBodyView earth),
                Is.True);

            CelestialLayeredBodyView layers = earth.LayeredBodyView;
            Assert.That(layers, Is.Not.Null);
            Assert.That(layers.IsInitialized, Is.True);
            Assert.That(layers.CloudShell.parent, Is.SameAs(earth.VisualRoot));
            Assert.That(layers.AtmosphereShell.parent, Is.SameAs(earth.VisualRoot));
            Assert.That(
                layers.CloudShell.localScale.x,
                Is.EqualTo(EarthLayerRenderingContract.CloudShellRadiusMultiplier)
                    .Within(0.0001f));
            Assert.That(
                layers.AtmosphereShell.localScale.x,
                Is.EqualTo(EarthLayerRenderingContract.AtmosphereShellRadiusMultiplier)
                    .Within(0.0001f));
            Assert.That(
                layers.SurfaceRenderer.sharedMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Earth Surface"));
            Assert.That(
                layers.CloudRenderer.sharedMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Earth Cloud Layer"));
            Assert.That(
                layers.AtmosphereRenderer.sharedMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Atmosphere Rim"));
            Assert.That(layers.CloudRenderer.shadowCastingMode, Is.EqualTo(
                ShadowCastingMode.Off));
            Assert.That(layers.AtmosphereRenderer.shadowCastingMode, Is.EqualTo(
                ShadowCastingMode.Off));

            SolarShaderGlobals globals =
                Object.FindAnyObjectByType<SolarShaderGlobals>();
            Assert.That(globals, Is.Not.Null);
            Assert.That(globals.IsInitialized, Is.True);
            Assert.That(globals.SunSource, Is.SameAs(sun.transform));
            yield return null;
            Vector4 shaderSun =
                Shader.GetGlobalVector(SolarShaderGlobals.SunPositionProperty);
            Assert.That(
                Vector3.Distance(
                    new Vector3(shaderSun.x, shaderSun.y, shaderSun.z),
                    sun.transform.position),
                Is.LessThan(0.0001f));

            Assert.That(EarthLayerRenderingContract.EvaluateNightWeight(1f), Is.Zero);
            Assert.That(
                EarthLayerRenderingContract.EvaluateNightWeight(-1f),
                Is.EqualTo(1f));
            Assert.That(
                layers.SurfaceRenderer.sharedMaterial.GetFloat("_NightFadeEnd"),
                Is.GreaterThan(
                    layers.SurfaceRenderer.sharedMaterial.GetFloat("_NightFadeStart")));

            float cloudAngleBefore = layers.CloudRelativeRotationDeg;
            yield return new WaitForSecondsRealtime(0.1f);
            Assert.That(
                Mathf.Abs(Mathf.DeltaAngle(
                    cloudAngleBefore,
                    layers.CloudRelativeRotationDeg)),
                Is.GreaterThan(0.01f));

            simulation.SimulationController.SetPaused(true);
            yield return null;
            float pausedCloudAngle = layers.CloudRelativeRotationDeg;
            yield return new WaitForSecondsRealtime(0.1f);
            Assert.That(
                Mathf.Abs(Mathf.DeltaAngle(
                    pausedCloudAngle,
                    layers.CloudRelativeRotationDeg)),
                Is.LessThan(0.0001f));
        }

        [UnityTest]
        public IEnumerator SolarSystemScene_UsesDeterministicSolarSurfaceAndCorona()
        {
            SceneManager.LoadScene("SolarSystem", LoadSceneMode.Single);
            yield return null;

            SolarSystemCompositionRoot simulation =
                Object.FindAnyObjectByType<SolarSystemCompositionRoot>();
            SolarSystemInteractionCompositionRoot interaction =
                Object.FindAnyObjectByType<SolarSystemInteractionCompositionRoot>();
            Assert.That(simulation, Is.Not.Null);
            Assert.That(interaction, Is.Not.Null);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "sun",
                    out CelestialBodyView sun),
                Is.True);

            SolarVisualView solar = sun.SolarVisualView;
            Assert.That(solar, Is.Not.Null);
            Assert.That(solar.IsInitialized, Is.True);
            Assert.That(solar.CoronaShell.parent, Is.SameAs(sun.VisualRoot));
            Assert.That(
                solar.CoronaShell.localScale.x,
                Is.EqualTo(SolarVisualRenderingContract.CoronaShellRadiusMultiplier)
                    .Within(0.0001f));
            Assert.That(
                solar.SurfaceRenderer.sharedMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Solar Surface"));
            Assert.That(
                solar.CoronaRenderer.sharedMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Solar Corona"));
            Assert.That(
                solar.SurfaceRenderer.shadowCastingMode,
                Is.EqualTo(ShadowCastingMode.Off));
            Assert.That(
                solar.CoronaRenderer.shadowCastingMode,
                Is.EqualTo(ShadowCastingMode.Off));
            Assert.That(solar.CoronaRenderer.receiveShadows, Is.False);
            Assert.That(
                solar.CoronaRenderer.lightProbeUsage,
                Is.EqualTo(LightProbeUsage.Off));
            Assert.That(
                solar.CoronaRenderer.reflectionProbeUsage,
                Is.EqualTo(ReflectionProbeUsage.Off));

            Light radialLight =
                GameObject.Find("Solar Radial Light")?.GetComponent<Light>();
            Assert.That(radialLight, Is.Not.Null);
            Assert.That(radialLight.transform.parent, Is.SameAs(sun.transform));
            Assert.That(
                Vector3.Distance(radialLight.transform.position, sun.transform.position),
                Is.LessThan(0.00001f));

            float surfacePhaseBefore = solar.SurfacePhase;
            float coronaPhaseBefore = solar.CoronaPhase;
            yield return new WaitForSecondsRealtime(0.2f);
            Assert.That(
                PhaseDistance(surfacePhaseBefore, solar.SurfacePhase),
                Is.GreaterThan(0.00001f));
            Assert.That(
                PhaseDistance(coronaPhaseBefore, solar.CoronaPhase),
                Is.GreaterThan(0.00001f));

            simulation.SimulationController.SetPaused(true);
            yield return null;
            float pausedSurfacePhase = solar.SurfacePhase;
            float pausedCoronaPhase = solar.CoronaPhase;
            yield return new WaitForSecondsRealtime(0.1f);
            Assert.That(
                PhaseDistance(pausedSurfacePhase, solar.SurfacePhase),
                Is.LessThan(0.000001f));
            Assert.That(
                PhaseDistance(pausedCoronaPhase, solar.CoronaPhase),
                Is.LessThan(0.000001f));

            interaction.SelectionController.Select(sun);
            interaction.CameraController.Focus(sun);
            yield return WaitUntilFocused(interaction.CameraController);
            yield return null;
            Assert.That(solar.SurfaceRenderer.enabled, Is.True);
            Assert.That(solar.CoronaRenderer.enabled, Is.True);
            Assert.That(simulation.SimulationController.ClockSnapshot.IsPaused, Is.True);

            interaction.CameraController.ReturnToFreeFlight();
            yield return null;
            Assert.That(
                interaction.CameraController.Mode,
                Is.EqualTo(SolarSystemCameraMode.FreeFlight));
            Assert.That(
                Vector3.Distance(radialLight.transform.position, sun.transform.position),
                Is.LessThan(0.00001f));

            var properties = new MaterialPropertyBlock();
            solar.SurfaceRenderer.GetPropertyBlock(properties);
            Assert.That(
                properties.GetFloat(Shader.PropertyToID("_SimulationPhase")),
                Is.EqualTo(solar.SurfacePhase).Within(0.000001f));
            solar.CoronaRenderer.GetPropertyBlock(properties);
            Assert.That(
                properties.GetFloat(Shader.PropertyToID("_SimulationPhase")),
                Is.EqualTo(solar.CoronaPhase).Within(0.000001f));
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

        private static float PhaseDistance(float first, float second)
        {
            return Mathf.Abs(Mathf.DeltaAngle(first * 360f, second * 360f)) / 360f;
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

        private static IEnumerator WaitUntilGuided(
            SolarSystemCameraController cameraController)
        {
            float deadline = Time.realtimeSinceStartup + FocusTransitionTimeoutSeconds;
            while (cameraController.Mode == SolarSystemCameraMode.GuidedTransition &&
                   Time.realtimeSinceStartup < deadline)
            {
                yield return null;
            }

            Assert.That(
                cameraController.Mode,
                Is.EqualTo(SolarSystemCameraMode.GuidedComparison),
                "Guided camera did not settle within the transition timeout.");
        }

        private static IEnumerator WaitUntilExplorerRestored(
            SolarSystemCameraController cameraController)
        {
            float deadline = Time.realtimeSinceStartup + FocusTransitionTimeoutSeconds;
            while (cameraController.IsGuidedComparisonActive &&
                   Time.realtimeSinceStartup < deadline)
            {
                yield return null;
            }

            Assert.That(
                cameraController.IsGuidedComparisonActive,
                Is.False,
                "Explorer camera was not restored within the transition timeout.");
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
