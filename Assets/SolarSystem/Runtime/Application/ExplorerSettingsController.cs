using System;
using Tanvir.SolarSystem.Audio;
using Tanvir.SolarSystem.Input;
using Tanvir.SolarSystem.Presentation.CelestialBodies;

namespace Tanvir.SolarSystem.Application
{
    /// <summary>
    /// Applies persisted explorer settings through existing narrow runtime services.
    /// </summary>
    public sealed class ExplorerSettingsController : IDisposable
    {
        private readonly SolarSystemInputAdapter input;
        private readonly AudioDirector audio;
        private readonly PresentationMotionPreferenceService motion;
        private readonly CelestialNavigationController navigation;
        private readonly CelestialOrbitPathVisibilityController orbitVisibility;
        private bool isApplying;
        private bool isDisposed;

        public ExplorerSettingsController(
            SolarSystemInputAdapter inputAdapter,
            AudioDirector audioDirector,
            PresentationMotionPreferenceService motionPreference,
            CelestialNavigationController navigationController,
            CelestialOrbitPathVisibilityController orbitPathVisibility,
            IExplorerSettingsStore store)
        {
            input = inputAdapter ?? throw new ArgumentNullException(nameof(inputAdapter));
            audio = audioDirector ?? throw new ArgumentNullException(nameof(audioDirector));
            motion = motionPreference ??
                throw new ArgumentNullException(nameof(motionPreference));
            navigation = navigationController ??
                throw new ArgumentNullException(nameof(navigationController));
            orbitVisibility = orbitPathVisibility ??
                throw new ArgumentNullException(nameof(orbitPathVisibility));
            if (!navigation.IsInitialized || !orbitVisibility.IsInitialized)
            {
                throw new InvalidOperationException(
                    "Navigation and orbit visibility must initialize before settings.");
            }

            Service = new ExplorerSettingsService(store);
            Service.Changed += OnSettingsChanged;
            motion.Changed += OnMotionChanged;
            navigation.Service.Changed += OnNavigationChanged;
            orbitVisibility.UserVisibilityChanged += OnOrbitVisibilityChanged;
            input.ToggleOrbitGuidesPerformed += ToggleOrbitGuides;
            Apply(Service.Current);
        }

        public ExplorerSettingsService Service { get; }

        public void SetMasterVolume(float value) => Service.SetMasterVolume(value);
        public void SetMusicVolume(float value) => Service.SetMusicVolume(value);
        public void SetUiVolume(float value) => Service.SetUiVolume(value);
        public void SetCelestialVolume(float value) => Service.SetCelestialVolume(value);
        public void SetMuted(bool value) => Service.SetMuted(value);
        public void SetMotionMode(PresentationMotionMode value) =>
            Service.SetMotionMode(value);
        public void SetOrbitGuidesEnabled(bool value) =>
            Service.SetOrbitGuidesEnabled(value);
        public void SetWorldLabelsEnabled(bool value) =>
            Service.SetWorldLabelsEnabled(value);
        public void CompleteOnboarding() => Service.CompleteOnboarding();
        public void ResetToDefaults() => Service.ResetToDefaults();

        public void ToggleOrbitGuides()
        {
            Service.SetOrbitGuidesEnabled(
                !Service.Current.AreOrbitGuidesEnabled);
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            Service.Changed -= OnSettingsChanged;
            motion.Changed -= OnMotionChanged;
            if (navigation.Service != null)
            {
                navigation.Service.Changed -= OnNavigationChanged;
            }

            orbitVisibility.UserVisibilityChanged -= OnOrbitVisibilityChanged;
            input.ToggleOrbitGuidesPerformed -= ToggleOrbitGuides;
            isDisposed = true;
        }

        private void OnSettingsChanged()
        {
            if (!isApplying)
            {
                Apply(Service.Current);
            }
        }

        private void Apply(ExplorerSettingsSnapshot settings)
        {
            isApplying = true;
            try
            {
                audio.SetMasterVolume(settings.MasterVolume);
                audio.SetMusicVolume(settings.MusicVolume);
                audio.SetUiVolume(settings.UiVolume);
                audio.SetCelestialVolume(settings.CelestialVolume);
                audio.SetMuted(settings.IsMuted);
                motion.SetMode(settings.MotionMode);
                navigation.SetWorldLabelsEnabled(settings.AreWorldLabelsEnabled);
                orbitVisibility.SetUserVisibility(settings.AreOrbitGuidesEnabled);
            }
            finally
            {
                isApplying = false;
            }
        }

        private void OnMotionChanged()
        {
            if (!isApplying)
            {
                PersistExternalChange(() => Service.SetMotionMode(motion.Mode));
            }
        }

        private void OnNavigationChanged()
        {
            if (!isApplying)
            {
                PersistExternalChange(
                    () => Service.SetWorldLabelsEnabled(
                        navigation.Service.AreWorldLabelsEnabled));
            }
        }

        private void OnOrbitVisibilityChanged()
        {
            if (!isApplying)
            {
                PersistExternalChange(
                    () => Service.SetOrbitGuidesEnabled(
                        orbitVisibility.ArePathsEnabledByUser));
            }
        }

        private void PersistExternalChange(Action change)
        {
            isApplying = true;
            try
            {
                change();
            }
            finally
            {
                isApplying = false;
            }
        }
    }
}
