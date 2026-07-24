using NUnit.Framework;
using Tanvir.SolarSystem.Application;
using Tanvir.SolarSystem.Audio;
using Tanvir.SolarSystem.Interaction;
using Tanvir.SolarSystem.Presentation.Camera;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
using Tanvir.SolarSystem.Simulation;
using UnityEditor;
using UnityEngine;

namespace Tanvir.SolarSystem.Tests.EditMode
{
    public sealed class AudioDirectorTests
    {
        private GameObject root;
        private AudioDirector director;
        private AudioSource music;
        private AudioSource sun;
        private AudioSource earth;
        private AudioSource ui;
        private AudioClip selectionClip;
        private AudioClip focusClip;
        private AudioClip timeClip;
        private SelectionService selection;
        private SimulationTimeControlService timeControls;
        private SolarSystemCameraController cameraController;

        [SetUp]
        public void SetUp()
        {
            root = new GameObject("Audio Director Test");
            director = root.AddComponent<AudioDirector>();
            music = CreateSource("Music");
            sun = CreateSource("Sun");
            earth = CreateSource("Earth");
            ui = CreateSource("UI");
            selectionClip = AudioClip.Create("Selection", 64, 1, 8000, false);
            focusClip = AudioClip.Create("Focus", 64, 1, 8000, false);
            timeClip = AudioClip.Create("Time", 64, 1, 8000, false);

            var serialized = new SerializedObject(director);
            serialized.FindProperty("musicSource").objectReferenceValue = music;
            serialized.FindProperty("sunAmbienceSource").objectReferenceValue = sun;
            serialized.FindProperty("earthAmbienceSource").objectReferenceValue = earth;
            serialized.FindProperty("uiSource").objectReferenceValue = ui;
            serialized.FindProperty("selectionClip").objectReferenceValue = selectionClip;
            serialized.FindProperty("focusClip").objectReferenceValue = focusClip;
            serialized.FindProperty("timeControlClip").objectReferenceValue = timeClip;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            selection = new SelectionService();
            timeControls = new SimulationTimeControlService(new FakeTimeController());
            cameraController = new GameObject("Camera Controller")
                .AddComponent<SolarSystemCameraController>();
            director.Initialize(selection, timeControls, cameraController);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(selectionClip);
            Object.DestroyImmediate(focusClip);
            Object.DestroyImmediate(timeClip);
            Object.DestroyImmediate(cameraController.gameObject);
            Object.DestroyImmediate(root);
        }

        [Test]
        public void Initialize_WithCompleteConfiguration_AppliesConsiderateDefaultMix()
        {
            Assert.That(director.IsInitialized, Is.True);
            Assert.That(music.volume, Is.EqualTo(0.117f).Within(0.0001f));
            Assert.That(ui.volume, Is.EqualTo(0.2925f).Within(0.0001f));
            Assert.That(sun.volume, Is.EqualTo(0.143f).Within(0.0001f));
            Assert.That(earth.volume, Is.EqualTo(0.143f).Within(0.0001f));
        }

        [Test]
        public void ApplicationEvents_RequestOnePurposeSpecificCueEach()
        {
            selection.Select(new CelestialBodyId("earth"));

            Assert.That(director.LastFeedbackCue, Is.EqualTo(AudioFeedbackCue.Selection));
            Assert.That(director.FeedbackCueCount, Is.EqualTo(1));

            CelestialBodyView view = new GameObject("Earth").AddComponent<CelestialBodyView>();
            try
            {
                cameraController.Focus(view);
                Assert.That(director.LastFeedbackCue, Is.EqualTo(AudioFeedbackCue.Focus));
                Assert.That(director.FeedbackCueCount, Is.EqualTo(2));

                timeControls.TogglePaused();
                Assert.That(
                    director.LastFeedbackCue,
                    Is.EqualTo(AudioFeedbackCue.TimeControl));
                Assert.That(director.FeedbackCueCount, Is.EqualTo(3));

                selection.Clear();
                Assert.That(director.FeedbackCueCount, Is.EqualTo(3));
            }
            finally
            {
                Object.DestroyImmediate(view.gameObject);
            }
        }

        [Test]
        public void ChannelAndMasterControls_RemainIndependentAndMutePreservesLevels()
        {
            director.SetMasterVolume(0.5f);
            director.SetMusicVolume(0.4f);
            director.SetUiVolume(0.6f);
            director.SetCelestialVolume(0.8f);

            Assert.That(music.volume, Is.EqualTo(0.2f).Within(0.0001f));
            Assert.That(ui.volume, Is.EqualTo(0.3f).Within(0.0001f));
            Assert.That(sun.volume, Is.EqualTo(0.4f).Within(0.0001f));
            Assert.That(earth.volume, Is.EqualTo(0.4f).Within(0.0001f));

            director.SetMuted(true);
            Assert.That(director.IsMuted, Is.True);
            Assert.That(music.mute, Is.True);
            Assert.That(ui.mute, Is.True);
            Assert.That(sun.mute, Is.True);
            Assert.That(earth.mute, Is.True);
            Assert.That(director.MusicVolume, Is.EqualTo(0.4f));

            director.SetMuted(false);
            Assert.That(music.mute, Is.False);
            Assert.That(music.volume, Is.EqualTo(0.2f).Within(0.0001f));
        }

        private AudioSource CreateSource(string name)
        {
            GameObject sourceObject = new GameObject(name);
            sourceObject.transform.SetParent(root.transform);
            return sourceObject.AddComponent<AudioSource>();
        }

        private sealed class FakeTimeController : ISimulationTimeController
        {
            private bool isPaused;
            private double speed = SimulationTimeControlService.BaselineSecondsPerRealSecond;

            public SimulationClockSnapshot ClockSnapshot =>
                new SimulationClockSnapshot(0d, isPaused, speed);

            public void SetPaused(bool paused)
            {
                isPaused = paused;
            }

            public void SetSpeedMultiplier(double speedMultiplier)
            {
                speed = speedMultiplier;
            }
        }
    }
}
