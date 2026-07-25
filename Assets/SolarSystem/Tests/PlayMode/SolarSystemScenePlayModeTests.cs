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
            "io",
            "europa",
            "ganymede",
            "callisto",
            "saturn",
            "titan",
            "uranus",
            "neptune",
            "triton"
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
            Assert.That(composition.SimulationController.CatalogCount, Is.EqualTo(16));
            Assert.That(composition.SimulationController.ViewCount, Is.EqualTo(16));
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
                Is.EqualTo(15));

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
                Object.FindObjectsByType<CelestialOrbitPathView>(),
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
                Object.FindObjectsByType<CelestialOrbitPathView>(),
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
        public IEnumerator SolarSystemScene_ToursAuthoredChaptersAndRestoresExplorerState()
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
            Assert.That(
                composition.SimulationController.TryGetView(
                    "earth",
                    out CelestialBodyView earth),
                Is.True);

            interaction.SelectionController.Select(earth);
            interaction.CameraController.Focus(earth);
            yield return WaitUntilFocused(interaction.CameraController);
            interaction.TimeControls.SetPresetIndex(2);
            interaction.Navigation.SetNavigatorVisible(true);
            interaction.Navigation.SetWorldLabelsEnabled(true);
            AudioDirector audio = interaction.AudioDirector;
            audio.SetMasterVolume(0.72f);
            audio.SetMusicVolume(0.31f);
            audio.SetUiVolume(0.42f);
            audio.SetCelestialVolume(0.53f);
            audio.SetMuted(true);
            yield return new WaitForEndOfFrame();

            Camera camera = Camera.main;
            Vector3 savedFocusOffset =
                camera.transform.position - earth.transform.position;
            Quaternion savedRotation = camera.transform.rotation;
            float savedNear = camera.nearClipPlane;
            float savedFar = camera.farClipPlane;
            int savedMultiplier = interaction.TimeControls.CurrentMultiplier;
            bool savedPaused = interaction.TimeControls.IsPaused;
            int savedFeedbackCount = audio.FeedbackCueCount;
            CinematicTourService tour = interaction.CinematicTour;
            CinematicTourController tourController =
                Object.FindAnyObjectByType<CinematicTourController>();

            Assert.That(tour, Is.Not.Null);
            Assert.That(tourController, Is.Not.Null);
            tourController.StartOrAdvance();
            Assert.That(tour.IsActive, Is.True);
            yield return WaitUntilCinematic(interaction.CameraController);
            yield return null;

            Assert.That(tour.CurrentChapter.StableId, Is.EqualTo("sun"));
            Assert.That(interaction.HudPresenter.IsCinematicTourVisible, Is.True);
            Assert.That(
                interaction.HudPresenter.CinematicTourTitleText,
                Is.EqualTo("OUR STAR"));
            Assert.That(interaction.Navigation.Service.IsNavigatorVisible, Is.False);
            Assert.That(interaction.Navigation.Service.AreWorldLabelsEnabled, Is.False);
            Assert.That(interaction.SelectionController.SelectedView, Is.SameAs(earth));
            Assert.That(interaction.TimeControls.CurrentMultiplier, Is.EqualTo(savedMultiplier));
            Assert.That(interaction.TimeControls.IsPaused, Is.EqualTo(savedPaused));

            interaction.ScaleComparison.Advance();
            Assert.That(interaction.ScaleComparison.IsActive, Is.False);
            Assert.That(tour.IsActive, Is.True);

            string[] expectedRemainingChapters =
            {
                "earth-moon",
                "jupiter-system",
                "saturn",
                "outer-system"
            };
            foreach (string expectedChapter in expectedRemainingChapters)
            {
                tourController.StartOrAdvance();
                yield return WaitUntilCinematic(interaction.CameraController);
                Assert.That(tour.CurrentChapter.StableId, Is.EqualTo(expectedChapter));
            }

            tourController.StartOrAdvance();
            yield return WaitUntilAnyGuidedPresentationRestored(
                interaction.CameraController);
            float interactionRestoreDeadline =
                Time.realtimeSinceStartup + FocusTransitionTimeoutSeconds;
            while ((!interaction.Navigation.Service.IsNavigatorVisible ||
                    !interaction.Navigation.Service.AreWorldLabelsEnabled) &&
                   Time.realtimeSinceStartup < interactionRestoreDeadline)
            {
                yield return null;
            }

            yield return new WaitForEndOfFrame();

            Assert.That(tour.IsActive, Is.False, "Tour state must complete.");
            Assert.That(
                interaction.HudPresenter.IsCinematicTourVisible,
                Is.False,
                "Tour HUD must hide after completion.");
            Assert.That(interaction.SelectionController.SelectedView, Is.SameAs(earth));
            Assert.That(interaction.CameraController.FocusedTarget, Is.SameAs(earth));
            Assert.That(
                interaction.CameraController.Mode,
                Is.EqualTo(SolarSystemCameraMode.Focused));
            Assert.That(
                Vector3.Distance(
                    camera.transform.position - earth.transform.position,
                    savedFocusOffset),
                Is.LessThan(0.001f));
            Assert.That(
                Quaternion.Angle(camera.transform.rotation, savedRotation),
                Is.LessThan(0.001f));
            Assert.That(camera.nearClipPlane, Is.EqualTo(savedNear).Within(0.001f));
            Assert.That(camera.farClipPlane, Is.EqualTo(savedFar).Within(0.001f));
            Assert.That(interaction.TimeControls.CurrentMultiplier, Is.EqualTo(savedMultiplier));
            Assert.That(
                interaction.TimeControls.IsPaused,
                Is.EqualTo(savedPaused),
                "Pause state must remain unchanged.");
            Assert.That(
                interaction.Navigation.Service.IsNavigatorVisible,
                Is.True,
                "Navigator visibility must restore after the camera settles.");
            Assert.That(
                interaction.Navigation.Service.AreWorldLabelsEnabled,
                Is.True,
                "World-label preference must restore after the camera settles.");
            Assert.That(audio.MasterVolume, Is.EqualTo(0.72f).Within(0.0001f));
            Assert.That(audio.MusicVolume, Is.EqualTo(0.31f).Within(0.0001f));
            Assert.That(audio.UiVolume, Is.EqualTo(0.42f).Within(0.0001f));
            Assert.That(audio.CelestialVolume, Is.EqualTo(0.53f).Within(0.0001f));
            Assert.That(audio.IsMuted, Is.True, "Audio mute must remain unchanged.");
            Assert.That(audio.FeedbackCueCount, Is.EqualTo(savedFeedbackCount));
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
            Assert.That(radialLight.range, Is.EqualTo(1000f).Within(0.001f));
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

        [UnityTest]
        public IEnumerator SolarSystemScene_UsesDeterministicJupiterHeroRendering()
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
                    "jupiter",
                    out CelestialBodyView jupiter),
                Is.True);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "earth",
                    out CelestialBodyView earth),
                Is.True);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "sun",
                    out CelestialBodyView sun),
                Is.True);

            GasGiantVisualView gasGiant = jupiter.GasGiantVisualView;
            Assert.That(gasGiant, Is.Not.Null);
            Assert.That(gasGiant.IsInitialized, Is.True);
            Assert.That(
                gasGiant.AtmosphereShell.parent,
                Is.SameAs(jupiter.VisualRoot));
            Assert.That(
                gasGiant.AtmosphereShell.localScale.x,
                Is.EqualTo(
                    GasGiantVisualRenderingContract.AtmosphereShellRadiusMultiplier)
                    .Within(0.0001f));
            Assert.That(
                gasGiant.SurfaceRenderer.sharedMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Giant Planet Surface"));
            Assert.That(
                gasGiant.AtmosphereRenderer.sharedMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Giant Planet Atmosphere"));
            Assert.That(
                gasGiant.SurfaceRenderer.shadowCastingMode,
                Is.EqualTo(ShadowCastingMode.Off));
            Assert.That(
                gasGiant.AtmosphereRenderer.shadowCastingMode,
                Is.EqualTo(ShadowCastingMode.Off));
            Assert.That(gasGiant.AtmosphereRenderer.receiveShadows, Is.False);
            Assert.That(
                gasGiant.AtmosphereRenderer.lightProbeUsage,
                Is.EqualTo(LightProbeUsage.Off));
            Assert.That(
                gasGiant.AtmosphereRenderer.reflectionProbeUsage,
                Is.EqualTo(ReflectionProbeUsage.Off));
            Assert.That(
                jupiter.CurrentDisplayRadius / earth.CurrentDisplayRadius,
                Is.EqualTo(69911f / 6371.0084f).Within(0.001f));

            Light radialLight =
                GameObject.Find("Solar Radial Light")?.GetComponent<Light>();
            Assert.That(radialLight, Is.Not.Null);
            AssertReceivesSunOriginLight(radialLight, sun, jupiter);

            float phaseBefore = gasGiant.BandPhase;
            yield return new WaitForSecondsRealtime(0.2f);
            Assert.That(
                PhaseDistance(phaseBefore, gasGiant.BandPhase),
                Is.GreaterThan(0.00001f));

            simulation.SimulationController.SetPaused(true);
            yield return null;
            float pausedPhase = gasGiant.BandPhase;
            yield return new WaitForSecondsRealtime(0.1f);
            Assert.That(
                PhaseDistance(pausedPhase, gasGiant.BandPhase),
                Is.LessThan(0.000001f));

            interaction.SelectionController.Select(jupiter);
            interaction.CameraController.Focus(jupiter);
            yield return WaitUntilFocused(interaction.CameraController);
            yield return null;
            Assert.That(gasGiant.SurfaceRenderer.enabled, Is.True);
            Assert.That(gasGiant.AtmosphereRenderer.enabled, Is.True);
            Assert.That(
                simulation.SimulationController.ClockSnapshot.IsPaused,
                Is.True);

            var properties = new MaterialPropertyBlock();
            gasGiant.SurfaceRenderer.GetPropertyBlock(properties);
            Assert.That(
                properties.GetFloat(Shader.PropertyToID("_SimulationPhase")),
                Is.EqualTo(gasGiant.BandPhase).Within(0.000001f));

            interaction.CameraController.ReturnToFreeFlight();
            yield return null;
            Assert.That(
                interaction.CameraController.Mode,
                Is.EqualTo(SolarSystemCameraMode.FreeFlight));
            AssertReceivesSunOriginLight(radialLight, sun, jupiter);
        }

        [UnityTest]
        public IEnumerator SolarSystemScene_UsesDeterministicSaturnHeroAndRings()
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
                    "saturn",
                    out CelestialBodyView saturn),
                Is.True);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "earth",
                    out CelestialBodyView earth),
                Is.True);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "sun",
                    out CelestialBodyView sun),
                Is.True);

            GasGiantVisualView gasGiant = saturn.GasGiantVisualView;
            Transform rings = saturn.VisualRoot.Find("Rings");
            Assert.That(gasGiant, Is.Not.Null);
            Assert.That(gasGiant.IsInitialized, Is.True);
            Assert.That(rings, Is.Not.Null);
            Assert.That(rings.parent, Is.SameAs(saturn.VisualRoot));
            Assert.That(
                gasGiant.AtmosphereShell.localScale.x,
                Is.EqualTo(
                    GasGiantVisualRenderingContract
                        .SaturnAtmosphereShellRadiusMultiplier)
                    .Within(0.0001f));
            Assert.That(
                saturn.CurrentDisplayRadius / earth.CurrentDisplayRadius,
                Is.EqualTo(58232f / 6371.0084f).Within(0.001f));

            MeshRenderer ringRenderer = rings.GetComponent<MeshRenderer>();
            MeshFilter ringFilter = rings.GetComponent<MeshFilter>();
            Assert.That(ringRenderer, Is.Not.Null);
            Assert.That(ringFilter, Is.Not.Null);
            Assert.That(
                ringRenderer.sharedMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Saturn Rings"));
            Assert.That(ringRenderer.shadowCastingMode, Is.EqualTo(ShadowCastingMode.Off));
            Assert.That(ringRenderer.receiveShadows, Is.False);
            Assert.That(ringRenderer.lightProbeUsage, Is.EqualTo(LightProbeUsage.Off));
            Assert.That(
                ringRenderer.reflectionProbeUsage,
                Is.EqualTo(ReflectionProbeUsage.Off));
            Assert.That(ringFilter.sharedMesh.vertexCount, Is.EqualTo(258));
            Assert.That(rings.localRotation, Is.EqualTo(Quaternion.identity));
            Assert.That(
                Vector3.Dot(rings.up, saturn.VisualRoot.up),
                Is.GreaterThan(0.99999f));

            Light radialLight =
                GameObject.Find("Solar Radial Light")?.GetComponent<Light>();
            Assert.That(radialLight, Is.Not.Null);
            AssertReceivesSunOriginLight(radialLight, sun, saturn);

            float phaseBefore = gasGiant.BandPhase;
            yield return new WaitForSecondsRealtime(0.2f);
            Assert.That(
                PhaseDistance(phaseBefore, gasGiant.BandPhase),
                Is.GreaterThan(0.00001f));
            simulation.SimulationController.SetPaused(true);
            yield return null;
            float pausedPhase = gasGiant.BandPhase;
            yield return new WaitForSecondsRealtime(0.1f);
            Assert.That(
                PhaseDistance(pausedPhase, gasGiant.BandPhase),
                Is.LessThan(0.000001f));

            interaction.SelectionController.Select(saturn);
            interaction.CameraController.Focus(saturn);
            yield return WaitUntilFocused(interaction.CameraController);
            yield return null;
            Assert.That(gasGiant.SurfaceRenderer.enabled, Is.True);
            Assert.That(gasGiant.AtmosphereRenderer.enabled, Is.True);
            Assert.That(ringRenderer.enabled, Is.True);
            Assert.That(simulation.SimulationController.ClockSnapshot.IsPaused, Is.True);

            var properties = new MaterialPropertyBlock();
            gasGiant.SurfaceRenderer.GetPropertyBlock(properties);
            Assert.That(
                properties.GetFloat(Shader.PropertyToID("_SimulationPhase")),
                Is.EqualTo(gasGiant.BandPhase).Within(0.000001f));

            interaction.CameraController.ReturnToFreeFlight();
            yield return null;
            Assert.That(
                interaction.CameraController.Mode,
                Is.EqualTo(SolarSystemCameraMode.FreeFlight));
            AssertReceivesSunOriginLight(radialLight, sun, saturn);
        }

        [UnityTest]
        public IEnumerator SolarSystemScene_UsesDistinctDeterministicIceGiantHeroes()
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
                    "uranus",
                    out CelestialBodyView uranus),
                Is.True);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "neptune",
                    out CelestialBodyView neptune),
                Is.True);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "earth",
                    out CelestialBodyView earth),
                Is.True);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "sun",
                    out CelestialBodyView sun),
                Is.True);

            IceGiantVisualView uranusVisual = uranus.IceGiantVisualView;
            IceGiantVisualView neptuneVisual = neptune.IceGiantVisualView;
            Assert.That(uranusVisual, Is.Not.Null);
            Assert.That(neptuneVisual, Is.Not.Null);
            Assert.That(uranusVisual.IsInitialized, Is.True);
            Assert.That(neptuneVisual.IsInitialized, Is.True);
            Assert.That(uranus.GasGiantVisualView, Is.Null);
            Assert.That(neptune.GasGiantVisualView, Is.Null);
            Assert.That(uranus.Definition.RotationPeriodSeconds, Is.LessThan(0d));
            Assert.That(neptune.Definition.RotationPeriodSeconds, Is.GreaterThan(0d));
            Assert.That(uranus.Definition.AxialTiltDeg, Is.EqualTo(97.77d));
            Assert.That(neptune.Definition.AxialTiltDeg, Is.EqualTo(28d));

            Assert.That(
                uranusVisual.AtmosphereShell.parent,
                Is.SameAs(uranus.VisualRoot));
            Assert.That(
                neptuneVisual.AtmosphereShell.parent,
                Is.SameAs(neptune.VisualRoot));
            Assert.That(
                uranusVisual.AtmosphereShell.localScale.x,
                Is.EqualTo(
                    IceGiantVisualRenderingContract
                        .UranusAtmosphereShellRadiusMultiplier)
                    .Within(0.0001f));
            Assert.That(
                neptuneVisual.AtmosphereShell.localScale.x,
                Is.EqualTo(
                    IceGiantVisualRenderingContract
                        .NeptuneAtmosphereShellRadiusMultiplier)
                    .Within(0.0001f));
            Assert.That(
                uranusVisual.SurfaceRenderer.sharedMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Giant Planet Surface"));
            Assert.That(
                neptuneVisual.SurfaceRenderer.sharedMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Giant Planet Surface"));
            Assert.That(
                uranusVisual.AtmosphereRenderer.sharedMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Giant Planet Atmosphere"));
            Assert.That(
                neptuneVisual.AtmosphereRenderer.sharedMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Giant Planet Atmosphere"));
            Assert.That(
                uranusVisual.SurfaceRenderer.sharedMaterial,
                Is.Not.SameAs(neptuneVisual.SurfaceRenderer.sharedMaterial));
            Assert.That(
                uranusVisual.AtmosphereRenderer.sharedMaterial,
                Is.Not.SameAs(neptuneVisual.AtmosphereRenderer.sharedMaterial));
            Assert.That(
                uranus.CurrentDisplayRadius / earth.CurrentDisplayRadius,
                Is.EqualTo(25362f / 6371.0084f).Within(0.001f));
            Assert.That(
                neptune.CurrentDisplayRadius / earth.CurrentDisplayRadius,
                Is.EqualTo(24622f / 6371.0084f).Within(0.001f));

            Light radialLight =
                GameObject.Find("Solar Radial Light")?.GetComponent<Light>();
            Assert.That(radialLight, Is.Not.Null);
            AssertReceivesSunOriginLight(radialLight, sun, uranus);
            AssertReceivesSunOriginLight(radialLight, sun, neptune);

            float uranusBefore = uranusVisual.DetailPhase;
            float neptuneBefore = neptuneVisual.DetailPhase;
            yield return new WaitForSecondsRealtime(0.25f);
            Assert.That(
                SignedPhaseDelta(uranusBefore, uranusVisual.DetailPhase),
                Is.LessThan(-0.00001f),
                "Uranus detail must follow its authoritative retrograde sign.");
            Assert.That(
                SignedPhaseDelta(neptuneBefore, neptuneVisual.DetailPhase),
                Is.GreaterThan(0.00001f),
                "Neptune detail must follow its authoritative prograde sign.");

            simulation.SimulationController.SetPaused(true);
            yield return null;
            float pausedUranus = uranusVisual.DetailPhase;
            float pausedNeptune = neptuneVisual.DetailPhase;
            yield return new WaitForSecondsRealtime(0.1f);
            Assert.That(
                PhaseDistance(pausedUranus, uranusVisual.DetailPhase),
                Is.LessThan(0.000001f));
            Assert.That(
                PhaseDistance(pausedNeptune, neptuneVisual.DetailPhase),
                Is.LessThan(0.000001f));

            interaction.SelectionController.Select(uranus);
            interaction.CameraController.Focus(uranus);
            yield return WaitUntilFocused(interaction.CameraController);
            yield return null;
            Assert.That(uranusVisual.SurfaceRenderer.enabled, Is.True);
            Assert.That(uranusVisual.AtmosphereRenderer.enabled, Is.True);
            interaction.CameraController.ReturnToFreeFlight();
            yield return null;

            interaction.SelectionController.Select(neptune);
            interaction.CameraController.Focus(neptune);
            yield return WaitUntilFocused(interaction.CameraController);
            yield return null;
            Assert.That(neptuneVisual.SurfaceRenderer.enabled, Is.True);
            Assert.That(neptuneVisual.AtmosphereRenderer.enabled, Is.True);
            Assert.That(
                simulation.SimulationController.ClockSnapshot.IsPaused,
                Is.True);
            interaction.CameraController.ReturnToFreeFlight();
            yield return null;

            var properties = new MaterialPropertyBlock();
            uranusVisual.SurfaceRenderer.GetPropertyBlock(properties);
            Assert.That(
                properties.GetFloat(Shader.PropertyToID("_SimulationPhase")),
                Is.EqualTo(uranusVisual.DetailPhase).Within(0.000001f));
            properties.Clear();
            neptuneVisual.SurfaceRenderer.GetPropertyBlock(properties);
            Assert.That(
                properties.GetFloat(Shader.PropertyToID("_SimulationPhase")),
                Is.EqualTo(neptuneVisual.DetailPhase).Within(0.000001f));
        }

        [UnityTest]
        public IEnumerator
            SolarSystemScene_UsesDistinctAirlessMercuryAndMoonHeroes()
        {
            SceneManager.LoadScene("SolarSystem", LoadSceneMode.Single);
            yield return null;

            SolarSystemCompositionRoot simulation =
                Object.FindAnyObjectByType<SolarSystemCompositionRoot>();
            Assert.That(simulation, Is.Not.Null);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "mercury",
                    out CelestialBodyView mercury),
                Is.True);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "moon",
                    out CelestialBodyView moon),
                Is.True);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "earth",
                    out CelestialBodyView earth),
                Is.True);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "sun",
                    out CelestialBodyView sun),
                Is.True);

            AirlessRockyVisualView mercuryVisual =
                mercury.AirlessRockyVisualView;
            AirlessRockyVisualView moonVisual = moon.AirlessRockyVisualView;
            Assert.That(mercuryVisual, Is.Not.Null);
            Assert.That(moonVisual, Is.Not.Null);
            Assert.That(mercuryVisual.IsInitialized, Is.True);
            Assert.That(moonVisual.IsInitialized, Is.True);
            Assert.That(mercury.LayeredBodyView, Is.Null);
            Assert.That(moon.LayeredBodyView, Is.Null);
            Assert.That(mercury.GasGiantVisualView, Is.Null);
            Assert.That(moon.GasGiantVisualView, Is.Null);
            Assert.That(mercury.IceGiantVisualView, Is.Null);
            Assert.That(moon.IceGiantVisualView, Is.Null);
            Assert.That(mercury.Definition.ParentId, Is.EqualTo("sun"));
            Assert.That(moon.Definition.ParentId, Is.EqualTo("earth"));
            Assert.That(mercury.Definition.HasOrbit, Is.True);
            Assert.That(moon.Definition.HasOrbit, Is.True);
            Assert.That(
                moon.Definition.Orbit.SemiMajorAxisKm,
                Is.EqualTo(384400d).Within(0.001d));
            Assert.That(
                mercury.Definition.RotationPeriodSeconds,
                Is.GreaterThan(0d));
            Assert.That(
                moon.Definition.RotationPeriodSeconds,
                Is.GreaterThan(0d));
            Assert.That(mercury.Definition.AxialTiltDeg, Is.EqualTo(2d));
            Assert.That(moon.Definition.AxialTiltDeg, Is.EqualTo(6.68d));

            Assert.That(
                mercuryVisual.SurfaceRenderer.sharedMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Rocky Surface"));
            Assert.That(
                moonVisual.SurfaceRenderer.sharedMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Rocky Surface"));
            Assert.That(
                mercuryVisual.SurfaceRenderer.sharedMaterial,
                Is.Not.SameAs(moonVisual.SurfaceRenderer.sharedMaterial));
            Assert.That(
                mercury.CurrentDisplayRadius / earth.CurrentDisplayRadius,
                Is.EqualTo(2439.4f / 6371.0084f).Within(0.0001f));
            Assert.That(
                moon.CurrentDisplayRadius / earth.CurrentDisplayRadius,
                Is.EqualTo(1737.4f / 6371.0084f).Within(0.0001f));

            var properties = new MaterialPropertyBlock();
            mercuryVisual.SurfaceRenderer.GetPropertyBlock(properties);
            Assert.That(
                properties.GetFloat(Shader.PropertyToID("_ReliefStrength")),
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract
                        .MercuryReliefStrength));
            Assert.That(
                properties.GetFloat(
                    Shader.PropertyToID("_NightsideReadability")),
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract
                        .MercuryNightsideReadability));
            moonVisual.SurfaceRenderer.GetPropertyBlock(properties);
            Assert.That(
                properties.GetFloat(Shader.PropertyToID("_ReliefStrength")),
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract.MoonReliefStrength));
            Assert.That(
                properties.GetFloat(
                    Shader.PropertyToID("_NightsideReadability")),
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract
                        .MoonNightsideReadability));

            Light radialLight =
                GameObject.Find("Solar Radial Light")?.GetComponent<Light>();
            Assert.That(radialLight, Is.Not.Null);
            AssertReceivesSunOriginLight(radialLight, sun, mercury);
            AssertReceivesSunOriginLight(radialLight, sun, moon);
        }

        [UnityTest]
        public IEnumerator
            SolarSystemScene_PresentsDistinctIoAndEuropaAirlessHeroes()
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
                    "io",
                    out CelestialBodyView io),
                Is.True);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "europa",
                    out CelestialBodyView europa),
                Is.True);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "earth",
                    out CelestialBodyView earth),
                Is.True);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "sun",
                    out CelestialBodyView sun),
                Is.True);

            AirlessRockyVisualView ioVisual = io.AirlessRockyVisualView;
            AirlessRockyVisualView europaVisual =
                europa.AirlessRockyVisualView;
            Assert.That(ioVisual, Is.Not.Null);
            Assert.That(europaVisual, Is.Not.Null);
            Assert.That(ioVisual.IsInitialized, Is.True);
            Assert.That(europaVisual.IsInitialized, Is.True);
            Assert.That(io.LayeredBodyView, Is.Null);
            Assert.That(europa.LayeredBodyView, Is.Null);
            Assert.That(io.GasGiantVisualView, Is.Null);
            Assert.That(europa.GasGiantVisualView, Is.Null);
            Assert.That(io.IceGiantVisualView, Is.Null);
            Assert.That(europa.IceGiantVisualView, Is.Null);
            Assert.That(io.Definition.ParentId, Is.EqualTo("jupiter"));
            Assert.That(europa.Definition.ParentId, Is.EqualTo("jupiter"));
            Assert.That(
                io.Definition.RotationPeriodSeconds,
                Is.EqualTo(1.762732d * 86400d).Within(0.001d));
            Assert.That(
                europa.Definition.RotationPeriodSeconds,
                Is.EqualTo(3.525463d * 86400d).Within(0.001d));
            Assert.That(io.Definition.RotationPeriodSeconds, Is.GreaterThan(0d));
            Assert.That(
                europa.Definition.RotationPeriodSeconds,
                Is.GreaterThan(0d));
            Assert.That(
                io.Definition.Orbit.SemiMajorAxisKm,
                Is.EqualTo(421800d));
            Assert.That(
                europa.Definition.Orbit.SemiMajorAxisKm,
                Is.EqualTo(671100d));
            Assert.That(
                io.CurrentDisplayRadius / earth.CurrentDisplayRadius,
                Is.EqualTo(1821.49f / 6371.0084f).Within(0.0001f));
            Assert.That(
                europa.CurrentDisplayRadius / earth.CurrentDisplayRadius,
                Is.EqualTo(1560.80f / 6371.0084f).Within(0.0001f));

            Material ioMaterial = ioVisual.SurfaceRenderer.sharedMaterial;
            Material europaMaterial =
                europaVisual.SurfaceRenderer.sharedMaterial;
            Assert.That(
                ioMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Rocky Surface"));
            Assert.That(
                europaMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Rocky Surface"));
            Assert.That(ioMaterial, Is.Not.SameAs(europaMaterial));
            Assert.That(
                ioMaterial.GetTexture("_BaseMap").name,
                Is.EqualTo("T_Io_Surface_Browse"));
            Assert.That(
                europaMaterial.GetTexture("_BaseMap").name,
                Is.EqualTo("T_Europa_Surface_Browse"));

            var properties = new MaterialPropertyBlock();
            ioVisual.SurfaceRenderer.GetPropertyBlock(properties);
            Assert.That(
                properties.GetFloat(Shader.PropertyToID("_ReliefStrength")),
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract.IoReliefStrength));
            Assert.That(
                properties.GetFloat(Shader.PropertyToID("_Smoothness")),
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract.IoSurfaceSmoothness));
            properties.Clear();
            europaVisual.SurfaceRenderer.GetPropertyBlock(properties);
            Assert.That(
                properties.GetFloat(Shader.PropertyToID("_ReliefStrength")),
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract
                        .EuropaReliefStrength));
            Assert.That(
                properties.GetFloat(Shader.PropertyToID("_Smoothness")),
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract
                        .EuropaSurfaceSmoothness));

            Assert.That(
                ioVisual.SurfaceRenderer.shadowCastingMode,
                Is.EqualTo(ShadowCastingMode.Off));
            Assert.That(ioVisual.SurfaceRenderer.receiveShadows, Is.False);
            Assert.That(
                europaVisual.SurfaceRenderer.shadowCastingMode,
                Is.EqualTo(ShadowCastingMode.Off));
            Assert.That(europaVisual.SurfaceRenderer.receiveShadows, Is.False);

            Vector4 globalSunPosition =
                Shader.GetGlobalVector("_SolarSystemSunPositionWS");
            Assert.That(
                Vector3.Distance(globalSunPosition, sun.transform.position),
                Is.LessThan(0.0001f));
            Light radialLight =
                GameObject.Find("Solar Radial Light")?.GetComponent<Light>();
            Assert.That(radialLight, Is.Not.Null);
            AssertReceivesSunOriginLight(radialLight, sun, io);
            AssertReceivesSunOriginLight(radialLight, sun, europa);

            bool wasPaused =
                simulation.SimulationController.ClockSnapshot.IsPaused;
            interaction.SelectionController.Select(io);
            Assert.That(
                interaction.SelectionController.SelectedView,
                Is.SameAs(io));
            interaction.CameraController.Focus(io);
            yield return WaitUntilFocused(interaction.CameraController);
            Assert.That(interaction.CameraController.FocusedTarget, Is.SameAs(io));

            interaction.SelectionController.Select(europa);
            interaction.CameraController.Focus(europa);
            yield return WaitUntilFocused(interaction.CameraController);
            Assert.That(
                interaction.SelectionController.SelectedView,
                Is.SameAs(europa));
            Assert.That(
                interaction.CameraController.FocusedTarget,
                Is.SameAs(europa));
            Assert.That(ioVisual.SurfaceRenderer.enabled, Is.True);
            Assert.That(europaVisual.SurfaceRenderer.enabled, Is.True);
            Assert.That(
                simulation.SimulationController.ClockSnapshot.IsPaused,
                Is.EqualTo(wasPaused));

            interaction.CameraController.ReturnToFreeFlight();
            yield return null;
        }

        [UnityTest]
        public IEnumerator
            SolarSystemScene_PresentsDistinctGanymedeAndCallistoHeroes()
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
                    "ganymede",
                    out CelestialBodyView ganymede),
                Is.True);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "callisto",
                    out CelestialBodyView callisto),
                Is.True);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "earth",
                    out CelestialBodyView earth),
                Is.True);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "sun",
                    out CelestialBodyView sun),
                Is.True);

            AirlessRockyVisualView ganymedeVisual =
                ganymede.AirlessRockyVisualView;
            AirlessRockyVisualView callistoVisual =
                callisto.AirlessRockyVisualView;
            Assert.That(ganymedeVisual, Is.Not.Null);
            Assert.That(callistoVisual, Is.Not.Null);
            Assert.That(ganymedeVisual.IsInitialized, Is.True);
            Assert.That(callistoVisual.IsInitialized, Is.True);
            Assert.That(ganymede.LayeredBodyView, Is.Null);
            Assert.That(callisto.LayeredBodyView, Is.Null);
            Assert.That(ganymede.GasGiantVisualView, Is.Null);
            Assert.That(callisto.GasGiantVisualView, Is.Null);
            Assert.That(ganymede.IceGiantVisualView, Is.Null);
            Assert.That(callisto.IceGiantVisualView, Is.Null);
            Assert.That(ganymede.Definition.ParentId, Is.EqualTo("jupiter"));
            Assert.That(callisto.Definition.ParentId, Is.EqualTo("jupiter"));
            Assert.That(
                ganymede.Definition.RotationPeriodSeconds,
                Is.EqualTo(7.155588d * 86400d).Within(0.001d));
            Assert.That(
                callisto.Definition.RotationPeriodSeconds,
                Is.EqualTo(16.690440d * 86400d).Within(0.001d));
            Assert.That(
                ganymede.Definition.RotationPeriodSeconds,
                Is.GreaterThan(0d));
            Assert.That(
                callisto.Definition.RotationPeriodSeconds,
                Is.GreaterThan(0d));
            Assert.That(
                ganymede.Definition.Orbit.SemiMajorAxisKm,
                Is.EqualTo(1070400d));
            Assert.That(
                callisto.Definition.Orbit.SemiMajorAxisKm,
                Is.EqualTo(1882700d));
            Assert.That(
                ganymede.CurrentDisplayRadius / earth.CurrentDisplayRadius,
                Is.EqualTo(2631.20f / 6371.0084f).Within(0.0001f));
            Assert.That(
                callisto.CurrentDisplayRadius / earth.CurrentDisplayRadius,
                Is.EqualTo(2410.30f / 6371.0084f).Within(0.0001f));

            Material ganymedeMaterial =
                ganymedeVisual.SurfaceRenderer.sharedMaterial;
            Material callistoMaterial =
                callistoVisual.SurfaceRenderer.sharedMaterial;
            Assert.That(
                ganymedeMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Rocky Surface"));
            Assert.That(
                callistoMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Rocky Surface"));
            Assert.That(ganymedeMaterial, Is.Not.SameAs(callistoMaterial));
            Assert.That(
                ganymedeMaterial.GetTexture("_BaseMap").name,
                Is.EqualTo("T_Ganymede_Surface_Browse"));
            Assert.That(
                callistoMaterial.GetTexture("_BaseMap").name,
                Is.EqualTo("T_Callisto_Surface_Browse"));

            var properties = new MaterialPropertyBlock();
            ganymedeVisual.SurfaceRenderer.GetPropertyBlock(properties);
            Assert.That(
                properties.GetFloat(Shader.PropertyToID("_ReliefStrength")),
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract
                        .GanymedeReliefStrength));
            Assert.That(
                properties.GetFloat(Shader.PropertyToID("_Smoothness")),
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract
                        .GanymedeSurfaceSmoothness));
            properties.Clear();
            callistoVisual.SurfaceRenderer.GetPropertyBlock(properties);
            Assert.That(
                properties.GetFloat(Shader.PropertyToID("_ReliefStrength")),
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract
                        .CallistoReliefStrength));
            Assert.That(
                properties.GetFloat(Shader.PropertyToID("_Smoothness")),
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract
                        .CallistoSurfaceSmoothness));

            Assert.That(
                ganymedeVisual.SurfaceRenderer.shadowCastingMode,
                Is.EqualTo(ShadowCastingMode.Off));
            Assert.That(
                ganymedeVisual.SurfaceRenderer.receiveShadows,
                Is.False);
            Assert.That(
                callistoVisual.SurfaceRenderer.shadowCastingMode,
                Is.EqualTo(ShadowCastingMode.Off));
            Assert.That(
                callistoVisual.SurfaceRenderer.receiveShadows,
                Is.False);

            Vector4 globalSunPosition =
                Shader.GetGlobalVector("_SolarSystemSunPositionWS");
            Assert.That(
                Vector3.Distance(globalSunPosition, sun.transform.position),
                Is.LessThan(0.0001f));
            Light radialLight =
                GameObject.Find("Solar Radial Light")?.GetComponent<Light>();
            Assert.That(radialLight, Is.Not.Null);
            AssertReceivesSunOriginLight(radialLight, sun, ganymede);
            AssertReceivesSunOriginLight(radialLight, sun, callisto);

            bool wasPaused =
                simulation.SimulationController.ClockSnapshot.IsPaused;
            interaction.SelectionController.Select(ganymede);
            yield return null;
            Assert.That(
                interaction.SelectionController.SelectedView,
                Is.SameAs(ganymede));
            Assert.That(
                interaction.HudPresenter.BodyNameText,
                Is.EqualTo("Ganymede"));
            Assert.That(
                interaction.HudPresenter.IsBodyInformationVisible,
                Is.True);
            interaction.CameraController.Focus(ganymede);
            yield return WaitUntilFocused(interaction.CameraController);
            Assert.That(
                interaction.CameraController.FocusedTarget,
                Is.SameAs(ganymede));

            interaction.SelectionController.Select(callisto);
            yield return null;
            Assert.That(
                interaction.HudPresenter.BodyNameText,
                Is.EqualTo("Callisto"));
            interaction.CameraController.Focus(callisto);
            yield return WaitUntilFocused(interaction.CameraController);
            Assert.That(
                interaction.SelectionController.SelectedView,
                Is.SameAs(callisto));
            Assert.That(
                interaction.CameraController.FocusedTarget,
                Is.SameAs(callisto));
            Assert.That(ganymedeVisual.SurfaceRenderer.enabled, Is.True);
            Assert.That(callistoVisual.SurfaceRenderer.enabled, Is.True);
            Assert.That(
                simulation.SimulationController.ClockSnapshot.IsPaused,
                Is.EqualTo(wasPaused));

            interaction.CameraController.ReturnToFreeFlight();
            yield return null;
        }

        [UnityTest]
        public IEnumerator SolarSystemScene_PresentsDistinctTritonHero()
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
                    "triton",
                    out CelestialBodyView triton),
                Is.True);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "earth",
                    out CelestialBodyView earth),
                Is.True);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "sun",
                    out CelestialBodyView sun),
                Is.True);

            AirlessRockyVisualView tritonVisual =
                triton.AirlessRockyVisualView;
            Assert.That(tritonVisual, Is.Not.Null);
            Assert.That(tritonVisual.IsInitialized, Is.True);
            Assert.That(triton.LayeredBodyView, Is.Null);
            Assert.That(triton.GasGiantVisualView, Is.Null);
            Assert.That(triton.IceGiantVisualView, Is.Null);
            Assert.That(triton.Definition.ParentId, Is.EqualTo("neptune"));
            Assert.That(
                triton.Definition.RotationPeriodSeconds,
                Is.EqualTo(-5.876994d * 86400d).Within(0.001d));
            Assert.That(
                triton.Definition.RotationPeriodSeconds,
                Is.LessThan(0d));
            Assert.That(
                triton.Definition.Orbit.OrbitalPeriodSeconds,
                Is.EqualTo(5.876994d * 86400d).Within(0.001d));
            Assert.That(
                triton.Definition.Orbit.InclinationDeg,
                Is.EqualTo(157.3d).Within(0.0001d));
            Assert.That(
                triton.Definition.Orbit.InclinationDeg,
                Is.GreaterThan(90d));
            Assert.That(
                triton.Definition.Orbit.SemiMajorAxisKm,
                Is.EqualTo(354800d));
            Assert.That(
                triton.CurrentDisplayRadius / earth.CurrentDisplayRadius,
                Is.EqualTo(1352.60f / 6371.0084f).Within(0.0001f));

            Material material = tritonVisual.SurfaceRenderer.sharedMaterial;
            Assert.That(
                material.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Rocky Surface"));
            Assert.That(
                material.GetTexture("_BaseMap").name,
                Is.EqualTo("T_Triton_Surface_Browse"));
            Assert.That(
                material.IsKeywordEnabled("_EMISSION"),
                Is.False);

            var properties = new MaterialPropertyBlock();
            tritonVisual.SurfaceRenderer.GetPropertyBlock(properties);
            Assert.That(
                properties.GetFloat(Shader.PropertyToID("_ReliefStrength")),
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract
                        .TritonReliefStrength));
            Assert.That(
                properties.GetFloat(
                    Shader.PropertyToID("_ReliefSampleDistance")),
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract
                        .TritonReliefSampleDistance));
            Assert.That(
                properties.GetFloat(Shader.PropertyToID("_Specular")),
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract
                        .TritonSurfaceSpecular));
            Assert.That(
                properties.GetFloat(Shader.PropertyToID("_Smoothness")),
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract
                        .TritonSurfaceSmoothness));
            Assert.That(
                properties.GetFloat(
                    Shader.PropertyToID("_NightsideReadability")),
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract
                        .TritonNightsideReadability));
            Assert.That(
                material.GetFloat("_CoverageFallbackStrength"),
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract
                        .TritonCoverageFallbackStrength));
            Assert.That(
                material.GetFloat("_CoverageThreshold"),
                Is.EqualTo(
                    AirlessRockyVisualRenderingContract
                        .TritonCoverageThreshold));

            Assert.That(
                tritonVisual.SurfaceRenderer.shadowCastingMode,
                Is.EqualTo(ShadowCastingMode.Off));
            Assert.That(
                tritonVisual.SurfaceRenderer.receiveShadows,
                Is.False);
            Assert.That(
                tritonVisual.SurfaceRenderer.lightProbeUsage,
                Is.EqualTo(LightProbeUsage.Off));
            Assert.That(
                tritonVisual.SurfaceRenderer.reflectionProbeUsage,
                Is.EqualTo(ReflectionProbeUsage.Off));
            Assert.That(
                triton.GetComponentsInChildren<Renderer>(true).Length,
                Is.EqualTo(1));

            Vector4 globalSunPosition =
                Shader.GetGlobalVector("_SolarSystemSunPositionWS");
            Assert.That(
                Vector3.Distance(globalSunPosition, sun.transform.position),
                Is.LessThan(0.0001f));
            Light radialLight =
                GameObject.Find("Solar Radial Light")?.GetComponent<Light>();
            Assert.That(radialLight, Is.Not.Null);
            AssertReceivesSunOriginLight(radialLight, sun, triton);
            Assert.That(
                Vector3.Distance(sun.transform.position, triton.transform.position),
                Is.LessThan(radialLight.range - 100f),
                "Triton must remain inside the authored light range with a stable culling margin.");

            bool wasPaused =
                simulation.SimulationController.ClockSnapshot.IsPaused;
            interaction.SelectionController.Select(triton);
            yield return null;
            Assert.That(
                interaction.SelectionController.SelectedView,
                Is.SameAs(triton));
            Assert.That(
                interaction.HudPresenter.BodyNameText,
                Is.EqualTo("Triton"));
            Assert.That(
                interaction.HudPresenter.IsBodyInformationVisible,
                Is.True);
            Assert.That(
                triton.Definition.EducationalSummary,
                Does.Contain("Voyager 2"));
            Assert.That(
                triton.Definition.EducationalSummary,
                Does.Contain("1989"));
            interaction.CameraController.Focus(triton);
            yield return WaitUntilFocused(interaction.CameraController);
            Assert.That(
                interaction.CameraController.FocusedTarget,
                Is.SameAs(triton));
            Assert.That(tritonVisual.SurfaceRenderer.enabled, Is.True);
            Assert.That(
                simulation.SimulationController.ClockSnapshot.IsPaused,
                Is.EqualTo(wasPaused));

            interaction.CameraController.ReturnToFreeFlight();
            yield return null;
        }

        [UnityTest]
        public IEnumerator SolarSystemScene_UsesDeterministicVenusCloudDeckAndLimb()
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
                    "venus",
                    out CelestialBodyView venus),
                Is.True);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "earth",
                    out CelestialBodyView earth),
                Is.True);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "sun",
                    out CelestialBodyView sun),
                Is.True);

            CelestialLayeredBodyView layers = venus.LayeredBodyView;
            Assert.That(layers, Is.Not.Null);
            Assert.That(layers.IsInitialized, Is.True);
            Assert.That(layers.CloudShell.parent, Is.SameAs(venus.VisualRoot));
            Assert.That(layers.AtmosphereShell.parent, Is.SameAs(venus.VisualRoot));
            Assert.That(
                layers.CloudShell.localScale.x,
                Is.EqualTo(VenusLayerRenderingContract.CloudShellRadiusMultiplier)
                    .Within(0.0001f));
            Assert.That(
                layers.AtmosphereShell.localScale.x,
                Is.EqualTo(
                    VenusLayerRenderingContract.AtmosphereShellRadiusMultiplier)
                    .Within(0.0001f));
            Assert.That(
                venus.CurrentDisplayRadius / earth.CurrentDisplayRadius,
                Is.EqualTo(6051.8f / 6371.0084f).Within(0.001f));
            Assert.That(
                layers.CloudRenderer.sharedMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Venus Cloud Deck"));
            Assert.That(
                layers.CloudRenderer.sharedMaterial.renderQueue,
                Is.EqualTo((int)RenderQueue.Geometry + 1));
            Assert.That(
                layers.AtmosphereRenderer.sharedMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Atmosphere Rim"));
            Assert.That(
                layers.CloudRenderer.shadowCastingMode,
                Is.EqualTo(ShadowCastingMode.Off));
            Assert.That(layers.CloudRenderer.receiveShadows, Is.False);
            Assert.That(
                layers.CloudRenderer.lightProbeUsage,
                Is.EqualTo(LightProbeUsage.Off));
            Assert.That(
                layers.CloudRenderer.reflectionProbeUsage,
                Is.EqualTo(ReflectionProbeUsage.Off));
            Assert.That(
                layers.AtmosphereRenderer.shadowCastingMode,
                Is.EqualTo(ShadowCastingMode.Off));

            Light radialLight =
                GameObject.Find("Solar Radial Light")?.GetComponent<Light>();
            Assert.That(radialLight, Is.Not.Null);
            AssertReceivesSunOriginLight(radialLight, sun, venus);
            Vector4 publishedSun =
                Shader.GetGlobalVector("_SolarSystemSunPositionWS");
            Assert.That(
                Vector3.Distance(publishedSun, sun.transform.position),
                Is.LessThan(0.0001f));

            float cloudAngleBefore = layers.CloudRelativeRotationDeg;
            yield return new WaitForSecondsRealtime(0.2f);
            float runningDelta =
                Mathf.DeltaAngle(
                    cloudAngleBefore,
                    layers.CloudRelativeRotationDeg);
            Assert.That(
                runningDelta,
                Is.GreaterThan(1f),
                "Venus's cloud deck should advance in its retrograde direction.");

            simulation.SimulationController.SetPaused(true);
            yield return null;
            float pausedAngle = layers.CloudRelativeRotationDeg;
            yield return new WaitForSecondsRealtime(0.1f);
            Assert.That(
                Mathf.Abs(
                    Mathf.DeltaAngle(
                        pausedAngle,
                        layers.CloudRelativeRotationDeg)),
                Is.LessThan(0.0001f));

            interaction.SelectionController.Select(venus);
            interaction.CameraController.Focus(venus);
            yield return WaitUntilFocused(interaction.CameraController);
            yield return null;
            Assert.That(layers.SurfaceRenderer.enabled, Is.True);
            Assert.That(layers.CloudRenderer.enabled, Is.True);
            Assert.That(layers.AtmosphereRenderer.enabled, Is.True);
            Assert.That(
                simulation.SimulationController.ClockSnapshot.IsPaused,
                Is.True);

            interaction.CameraController.ReturnToFreeFlight();
            yield return null;
            Assert.That(
                interaction.CameraController.Mode,
                Is.EqualTo(SolarSystemCameraMode.FreeFlight));
            AssertReceivesSunOriginLight(radialLight, sun, venus);
        }

        [UnityTest]
        public IEnumerator SolarSystemScene_UsesAnchoredMarsSurfaceAndThinAtmosphere()
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
                    "mars",
                    out CelestialBodyView mars),
                Is.True,
                "The runtime catalog must expose the authored Mars view.");
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "earth",
                    out CelestialBodyView earth),
                Is.True,
                "The Earth reference view is required for proportional validation.");
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "sun",
                    out CelestialBodyView sun),
                Is.True,
                "The Sun reference view is required for lighting validation.");

            CelestialLayeredBodyView layers = mars.LayeredBodyView;
            Assert.That(layers, Is.Not.Null);
            Assert.That(
                layers.IsInitialized,
                Is.True,
                "Mars's layered adapter must initialize with the simulation.");
            Assert.That(layers.HasCloudLayer, Is.False);
            Assert.That(layers.CloudShell, Is.Null);
            Assert.That(layers.CloudRenderer, Is.Null);
            Assert.That(layers.AtmosphereShell.parent, Is.SameAs(mars.VisualRoot));
            Assert.That(
                layers.AtmosphereShell.localScale.x,
                Is.EqualTo(MarsLayerRenderingContract.AtmosphereShellRadiusMultiplier)
                    .Within(0.0001f));
            Assert.That(
                mars.CurrentDisplayRadius / earth.CurrentDisplayRadius,
                Is.EqualTo(3389.5f / 6371.0084f).Within(0.001f));
            Assert.That(
                layers.SurfaceRenderer.sharedMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Rocky Surface"));
            Assert.That(
                layers.AtmosphereRenderer.sharedMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Atmosphere Rim"));
            Assert.That(
                layers.AtmosphereRenderer.shadowCastingMode,
                Is.EqualTo(ShadowCastingMode.Off));
            Assert.That(
                layers.AtmosphereRenderer.receiveShadows,
                Is.False,
                "The presentation atmosphere must not receive shadows.");
            Assert.That(
                layers.AtmosphereRenderer.lightProbeUsage,
                Is.EqualTo(LightProbeUsage.Off));
            Assert.That(
                layers.AtmosphereRenderer.reflectionProbeUsage,
                Is.EqualTo(ReflectionProbeUsage.Off));

            Light radialLight =
                GameObject.Find("Solar Radial Light")?.GetComponent<Light>();
            Assert.That(radialLight, Is.Not.Null);
            AssertReceivesSunOriginLight(radialLight, sun, mars);

            bool wasPausedBeforeFocus =
                simulation.SimulationController.ClockSnapshot.IsPaused;
            interaction.SelectionController.Select(mars);
            interaction.CameraController.Focus(mars);
            yield return WaitUntilFocused(interaction.CameraController);
            yield return null;
            Assert.That(
                layers.SurfaceRenderer.enabled,
                Is.True,
                "Mars's anchored surface must remain visible in close focus.");
            Assert.That(
                layers.AtmosphereRenderer.enabled,
                Is.True,
                "Mars's thin atmosphere must remain visible in close focus.");
            Assert.That(
                simulation.SimulationController.ClockSnapshot.IsPaused,
                Is.EqualTo(wasPausedBeforeFocus),
                "Visual focus must preserve the user's simulation-time state.");

            interaction.CameraController.ReturnToFreeFlight();
            yield return null;
            Assert.That(
                interaction.CameraController.Mode,
                Is.EqualTo(SolarSystemCameraMode.FreeFlight));
            AssertReceivesSunOriginLight(radialLight, sun, mars);
        }

        [UnityTest]
        public IEnumerator SolarSystemScene_PresentsTitanAsHazeDominantScientificMoon()
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
                    "titan",
                    out CelestialBodyView titan),
                Is.True);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "earth",
                    out CelestialBodyView earth),
                Is.True);
            Assert.That(
                simulation.SimulationController.TryGetView(
                    "sun",
                    out CelestialBodyView sun),
                Is.True);

            CelestialLayeredBodyView layers = titan.LayeredBodyView;
            Assert.That(layers, Is.Not.Null);
            Assert.That(layers.IsInitialized, Is.True);
            Assert.That(layers.HasCloudLayer, Is.False);
            Assert.That(layers.CloudShell, Is.Null);
            Assert.That(layers.CloudRenderer, Is.Null);
            Assert.That(layers.AtmosphereShell.parent, Is.SameAs(titan.VisualRoot));
            Assert.That(
                layers.AtmosphereShell.localScale.x,
                Is.EqualTo(TitanHazeRenderingContract.AtmosphereShellRadiusMultiplier)
                    .Within(0.0001f));
            Assert.That(
                titan.CurrentDisplayRadius / earth.CurrentDisplayRadius,
                Is.EqualTo(2574.76f / 6371.0084f).Within(0.0001f));
            Assert.That(titan.Definition.ParentId, Is.EqualTo("saturn"));
            Assert.That(titan.Definition.MeanRadiusKm, Is.EqualTo(2574.76d));
            Assert.That(
                titan.Definition.RotationPeriodSeconds,
                Is.EqualTo(15.945448d * 86400d).Within(0.001d));
            Assert.That(
                titan.Definition.Orbit.SemiMajorAxisKm,
                Is.EqualTo(1221900d));
            Assert.That(titan.Definition.Orbit.Eccentricity, Is.EqualTo(0.029d));
            Assert.That(
                layers.SurfaceRenderer.sharedMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Titan Surface"));
            Assert.That(
                layers.SurfaceRenderer.sharedMaterial
                    .GetTexture("_BaseMap").name,
                Is.EqualTo("T_Titan_Surface_Browse"));
            Assert.That(
                layers.AtmosphereRenderer.sharedMaterial.shader.name,
                Is.EqualTo("SolarSystem/Celestial/Titan Haze"));
            Assert.That(
                layers.AtmosphereRenderer.shadowCastingMode,
                Is.EqualTo(ShadowCastingMode.Off));
            Assert.That(layers.AtmosphereRenderer.receiveShadows, Is.False);
            Assert.That(
                layers.AtmosphereRenderer.lightProbeUsage,
                Is.EqualTo(LightProbeUsage.Off));
            Assert.That(
                layers.AtmosphereRenderer.reflectionProbeUsage,
                Is.EqualTo(ReflectionProbeUsage.Off));

            layers.Apply(123456d);
            float firstPhase = layers.AtmospherePhase;
            layers.Apply(123456d);
            Assert.That(
                layers.AtmospherePhase,
                Is.EqualTo(firstPhase).Within(0.000001f));
            layers.Apply(246912d);
            Assert.That(
                PhaseDistance(firstPhase, layers.AtmospherePhase),
                Is.GreaterThan(0.0001f));

            Vector4 globalSunPosition =
                Shader.GetGlobalVector("_SolarSystemSunPositionWS");
            Assert.That(
                Vector3.Distance(globalSunPosition, sun.transform.position),
                Is.LessThan(0.0001f));
            Light radialLight =
                GameObject.Find("Solar Radial Light")?.GetComponent<Light>();
            Assert.That(radialLight, Is.Not.Null);
            AssertReceivesSunOriginLight(radialLight, sun, titan);

            bool wasPausedBeforeFocus =
                simulation.SimulationController.ClockSnapshot.IsPaused;
            interaction.SelectionController.Select(titan);
            interaction.CameraController.Focus(titan);
            yield return WaitUntilFocused(interaction.CameraController);
            yield return null;
            Assert.That(layers.SurfaceRenderer.enabled, Is.True);
            Assert.That(layers.AtmosphereRenderer.enabled, Is.True);
            Assert.That(
                simulation.SimulationController.ClockSnapshot.IsPaused,
                Is.EqualTo(wasPausedBeforeFocus));

            interaction.CameraController.ReturnToFreeFlight();
            yield return null;
            Assert.That(
                interaction.CameraController.Mode,
                Is.EqualTo(SolarSystemCameraMode.FreeFlight));
        }

        [UnityTest]
        public IEnumerator SolarSystemScene_NavigatorListsAndFocusesAllAuthoredBodies()
        {
            SceneManager.LoadScene("SolarSystem", LoadSceneMode.Single);
            yield return null;

            SolarSystemInteractionCompositionRoot interaction =
                Object.FindAnyObjectByType<SolarSystemInteractionCompositionRoot>();
            Assert.That(interaction, Is.Not.Null);
            Assert.That(interaction.IsInitialized, Is.True);
            Assert.That(interaction.Navigation, Is.Not.Null);
            Assert.That(interaction.Navigation.IsInitialized, Is.True);

            SolarSystemHudPresenter hud = interaction.HudPresenter;
            Assert.That(hud.NavigatorEntryCount, Is.EqualTo(ExpectedBodyIds.Length));
            for (int index = 0; index < ExpectedBodyIds.Length; index++)
            {
                Assert.That(
                    hud.GetNavigatorEntryId(index),
                    Is.EqualTo(ExpectedBodyIds[index]),
                    "Navigator order must remain deterministic and parent-first.");
            }

            bool wasPaused = interaction.TimeControls.IsPaused;
            hud.SetNavigatorVisible(true);
            yield return null;
            Assert.That(hud.IsNavigatorVisible, Is.True);

            Assert.That(hud.NavigateTo("moon"), Is.True);
            yield return WaitUntilFocused(interaction.CameraController);
            yield return null;

            Assert.That(hud.IsNavigatorVisible, Is.False);
            Assert.That(
                interaction.SelectionController.SelectedView.StableId,
                Is.EqualTo("moon"));
            Assert.That(
                interaction.CameraController.FocusedTarget.StableId,
                Is.EqualTo("moon"));
            Assert.That(hud.BodyNameText, Is.EqualTo("Moon"));
            Assert.That(interaction.TimeControls.IsPaused, Is.EqualTo(wasPaused));
            Assert.That(hud.NavigateTo("not-a-body"), Is.False);
        }

        [UnityTest]
        public IEnumerator SolarSystemScene_LabelsRespectFocusGuidanceAndResponsiveSafeAreas()
        {
            SceneManager.LoadScene("SolarSystem", LoadSceneMode.Single);
            yield return null;
            yield return null;

            SolarSystemCompositionRoot composition =
                Object.FindAnyObjectByType<SolarSystemCompositionRoot>();
            SolarSystemInteractionCompositionRoot interaction =
                Object.FindAnyObjectByType<SolarSystemInteractionCompositionRoot>();
            Assert.That(composition, Is.Not.Null);
            Assert.That(interaction, Is.Not.Null);
            SolarSystemHudPresenter hud = interaction.HudPresenter;
            Assert.That(hud.AreWorldLabelsEnabled, Is.True);
            Assert.That(hud.WorldLabelCount, Is.EqualTo(ExpectedBodyIds.Length));

            Assert.That(
                composition.SimulationController.TryGetView(
                    "sun",
                    out CelestialBodyView sun),
                Is.True);
            interaction.SelectionController.Select(sun);
            yield return null;
            Assert.That(hud.VisibleWorldLabelCount, Is.GreaterThan(0));
            Assert.That(
                hud.VisibleWorldLabelCount,
                Is.LessThanOrEqualTo(ExpectedBodyIds.Length));
            Assert.That(hud.IsWorldLabelVisible("sun"), Is.True);

            interaction.CameraController.Focus(sun);
            yield return WaitUntilFocused(interaction.CameraController);
            yield return null;
            Assert.That(hud.VisibleWorldLabelCount, Is.EqualTo(1));
            Assert.That(hud.IsWorldLabelVisible("sun"), Is.True);

            interaction.CameraController.ReturnToFreeFlight();
            yield return null;
            hud.SetWorldLabelsEnabled(false);
            yield return null;
            Assert.That(hud.VisibleWorldLabelCount, Is.Zero);
            Assert.That(hud.LabelsStateText, Does.Contain("OFF"));

            hud.SetWorldLabelsEnabled(true);
            hud.SetNavigatorVisible(true);
            yield return null;
            Rect rootBounds = hud.HudWorldBound;
            Rect navigatorBounds = hud.NavigatorWorldBound;
            Assert.That(navigatorBounds.width, Is.GreaterThan(0f));
            Assert.That(navigatorBounds.height, Is.GreaterThan(0f));
            Assert.That(
                navigatorBounds.xMin,
                Is.GreaterThanOrEqualTo(rootBounds.xMin));
            Assert.That(
                navigatorBounds.yMin,
                Is.GreaterThanOrEqualTo(rootBounds.yMin));
            Assert.That(
                navigatorBounds.xMax,
                Is.LessThanOrEqualTo(rootBounds.xMax));
            Assert.That(
                navigatorBounds.yMax,
                Is.LessThanOrEqualTo(rootBounds.yMax));

            GuidedScaleComparisonService comparison = interaction.ScaleComparison;
            comparison.Advance();
            yield return WaitUntilGuided(interaction.CameraController);
            yield return null;
            Assert.That(hud.IsNavigatorVisible, Is.False);
            Assert.That(hud.VisibleWorldLabelCount, Is.Zero);
            hud.SetNavigatorVisible(true);
            Assert.That(hud.IsNavigatorVisible, Is.False);

            Assert.That(comparison.Cancel(), Is.True);
            yield return WaitUntilExplorerRestored(interaction.CameraController);
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

        private static float SignedPhaseDelta(float first, float second)
        {
            return Mathf.DeltaAngle(first * 360f, second * 360f) / 360f;
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

        private static IEnumerator WaitUntilCinematic(
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
                Is.EqualTo(SolarSystemCameraMode.CinematicTour),
                "Cinematic camera did not settle within the transition timeout.");
        }

        private static IEnumerator WaitUntilAnyGuidedPresentationRestored(
            SolarSystemCameraController cameraController)
        {
            float deadline = Time.realtimeSinceStartup + FocusTransitionTimeoutSeconds;
            while (cameraController.IsGuidedPresentationActive &&
                   Time.realtimeSinceStartup < deadline)
            {
                yield return null;
            }

            Assert.That(
                cameraController.IsGuidedPresentationActive,
                Is.False,
                "Explorer camera was not restored within the transition timeout.");
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
