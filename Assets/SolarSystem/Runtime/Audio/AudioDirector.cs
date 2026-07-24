using System;
using Tanvir.SolarSystem.Application;
using Tanvir.SolarSystem.Interaction;
using Tanvir.SolarSystem.Presentation.Camera;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
using Tanvir.SolarSystem.Simulation;
using UnityEngine;

namespace Tanvir.SolarSystem.Audio
{
    /// <summary>
    /// Routes licensed ambience and event-driven interaction feedback without
    /// observing simulation transforms or owning gameplay state.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AudioDirector : MonoBehaviour
    {
        [Header("Channels")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sunAmbienceSource;
        [SerializeField] private AudioSource earthAmbienceSource;
        [SerializeField] private AudioSource uiSource;

        [Header("Interface Cues")]
        [SerializeField] private AudioClip selectionClip;
        [SerializeField] private AudioClip focusClip;
        [SerializeField] private AudioClip timeControlClip;

        [Header("Default Mix")]
        [SerializeField, Range(0f, 1f)] private float masterVolume = 0.65f;
        [SerializeField, Range(0f, 1f)] private float musicVolume = 0.18f;
        [SerializeField, Range(0f, 1f)] private float uiVolume = 0.45f;
        [SerializeField, Range(0f, 1f)] private float celestialVolume = 0.22f;

        private SelectionService selection;
        private SimulationTimeControlService timeControls;
        private SolarSystemCameraController cameraController;

        /// <summary>Gets whether all serialized channels and event sources are ready.</summary>
        public bool IsInitialized =>
            selection != null &&
            timeControls != null &&
            cameraController != null &&
            HasCompleteConfiguration;

        /// <summary>Gets whether all required sources and clips are assigned.</summary>
        public bool HasCompleteConfiguration =>
            musicSource != null &&
            sunAmbienceSource != null &&
            earthAmbienceSource != null &&
            uiSource != null &&
            selectionClip != null &&
            focusClip != null &&
            timeControlClip != null;

        /// <summary>Gets the independently adjustable master level.</summary>
        public float MasterVolume => masterVolume;

        /// <summary>Gets the independently adjustable music level.</summary>
        public float MusicVolume => musicVolume;

        /// <summary>Gets the independently adjustable interface level.</summary>
        public float UiVolume => uiVolume;

        /// <summary>Gets the independently adjustable celestial-ambience level.</summary>
        public float CelestialVolume => celestialVolume;

        /// <summary>Gets whether all audio channels are muted.</summary>
        public bool IsMuted { get; private set; }

        /// <summary>Gets the last event-driven cue for diagnostics and validation.</summary>
        public AudioFeedbackCue LastFeedbackCue { get; private set; }

        /// <summary>Gets the number of event-driven cues requested this session.</summary>
        public int FeedbackCueCount { get; private set; }

        /// <summary>Gets the authored music source.</summary>
        public AudioSource MusicSource => musicSource;

        /// <summary>Gets the authored global Sun ambience source.</summary>
        public AudioSource SunAmbienceSource => sunAmbienceSource;

        /// <summary>Gets the authored spatial Earth ambience source.</summary>
        public AudioSource EarthAmbienceSource => earthAmbienceSource;

        /// <summary>Gets the authored non-spatial interface source.</summary>
        public AudioSource UiSource => uiSource;

        /// <summary>Subscribes the audio layer to existing application events.</summary>
        public void Initialize(
            SelectionService selectionService,
            SimulationTimeControlService simulationTimeControls,
            SolarSystemCameraController explorerCameraController)
        {
            if (!HasCompleteConfiguration)
            {
                throw new InvalidOperationException(
                    "Audio director requires every channel source and interface cue.");
            }

            Unsubscribe();
            selection = selectionService ??
                throw new ArgumentNullException(nameof(selectionService));
            timeControls = simulationTimeControls ??
                throw new ArgumentNullException(nameof(simulationTimeControls));
            cameraController = explorerCameraController ??
                throw new ArgumentNullException(nameof(explorerCameraController));

            selection.SelectionChanged += OnSelectionChanged;
            timeControls.Changed += OnTimeControlsChanged;
            cameraController.FocusStarted += OnFocusStarted;
            ApplyMix();
        }

        /// <summary>Sets the normalized master level without changing channel balance.</summary>
        public void SetMasterVolume(float normalizedVolume)
        {
            masterVolume = ClampVolume(normalizedVolume);
            ApplyMix();
        }

        /// <summary>Sets the normalized music channel level.</summary>
        public void SetMusicVolume(float normalizedVolume)
        {
            musicVolume = ClampVolume(normalizedVolume);
            ApplyMix();
        }

        /// <summary>Sets the normalized interface channel level.</summary>
        public void SetUiVolume(float normalizedVolume)
        {
            uiVolume = ClampVolume(normalizedVolume);
            ApplyMix();
        }

        /// <summary>Sets the normalized shared celestial-ambience channel level.</summary>
        public void SetCelestialVolume(float normalizedVolume)
        {
            celestialVolume = ClampVolume(normalizedVolume);
            ApplyMix();
        }

        /// <summary>Mutes or restores all channels while preserving their levels.</summary>
        public void SetMuted(bool muted)
        {
            IsMuted = muted;
            ApplyMix();
        }

        private static float ClampVolume(float value) => Mathf.Clamp01(value);

        private void ApplyMix()
        {
            if (musicSource == null ||
                sunAmbienceSource == null ||
                earthAmbienceSource == null ||
                uiSource == null)
            {
                return;
            }

            musicSource.volume = masterVolume * musicVolume;
            uiSource.volume = masterVolume * uiVolume;
            float celestialGain = masterVolume * celestialVolume;
            sunAmbienceSource.volume = celestialGain;
            earthAmbienceSource.volume = celestialGain;

            musicSource.mute = IsMuted;
            sunAmbienceSource.mute = IsMuted;
            earthAmbienceSource.mute = IsMuted;
            uiSource.mute = IsMuted;
        }

        private void OnSelectionChanged(CelestialBodyId? selectedId)
        {
            if (selectedId.HasValue)
            {
                PlayFeedback(selectionClip, AudioFeedbackCue.Selection);
            }
        }

        private void OnFocusStarted(CelestialBodyView target)
        {
            PlayFeedback(focusClip, AudioFeedbackCue.Focus);
        }

        private void OnTimeControlsChanged()
        {
            PlayFeedback(timeControlClip, AudioFeedbackCue.TimeControl);
        }

        private void PlayFeedback(AudioClip clip, AudioFeedbackCue cue)
        {
            uiSource.PlayOneShot(clip);
            LastFeedbackCue = cue;
            FeedbackCueCount++;
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            if (selection != null)
            {
                selection.SelectionChanged -= OnSelectionChanged;
            }

            if (timeControls != null)
            {
                timeControls.Changed -= OnTimeControlsChanged;
            }

            if (cameraController != null)
            {
                cameraController.FocusStarted -= OnFocusStarted;
            }

            selection = null;
            timeControls = null;
            cameraController = null;
        }
    }
}
