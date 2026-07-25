using System;

namespace Tanvir.SolarSystem.Application
{
    /// <summary>Persistence boundary for the unified explorer settings record.</summary>
    public interface IExplorerSettingsStore
    {
        bool TryLoad(out ExplorerSettingsSnapshot settings);
        void Save(ExplorerSettingsSnapshot settings);
    }

    /// <summary>Owns validated, version-independent player-facing settings state.</summary>
    public sealed class ExplorerSettingsService
    {
        private readonly IExplorerSettingsStore store;

        /// <summary>Creates the service and loads a valid saved snapshot when available.</summary>
        public ExplorerSettingsService(IExplorerSettingsStore settingsStore)
        {
            store = settingsStore ?? throw new ArgumentNullException(nameof(settingsStore));
            Current = store.TryLoad(out ExplorerSettingsSnapshot saved)
                ? saved
                : ExplorerSettingsSnapshot.CreateDefaults();
        }

        public event Action Changed;

        public ExplorerSettingsSnapshot Current { get; private set; }

        public void SetMasterVolume(float value) => Apply(Create(masterVolume: value));
        public void SetMusicVolume(float value) => Apply(Create(musicVolume: value));
        public void SetUiVolume(float value) => Apply(Create(uiVolume: value));
        public void SetCelestialVolume(float value) => Apply(Create(celestialVolume: value));
        public void SetMuted(bool value) => Apply(Create(isMuted: value));
        public void SetMotionMode(PresentationMotionMode value) =>
            Apply(Create(motionMode: value));
        public void SetOrbitGuidesEnabled(bool value) =>
            Apply(Create(areOrbitGuidesEnabled: value));
        public void SetWorldLabelsEnabled(bool value) =>
            Apply(Create(areWorldLabelsEnabled: value));
        public void CompleteOnboarding() =>
            Apply(Create(hasCompletedOnboarding: true));

        /// <summary>Restores release defaults without replaying first-launch onboarding.</summary>
        public void ResetToDefaults()
        {
            Apply(ExplorerSettingsSnapshot.CreateDefaults(
                Current.HasCompletedOnboarding));
        }

        private ExplorerSettingsSnapshot Create(
            float? masterVolume = null,
            float? musicVolume = null,
            float? uiVolume = null,
            float? celestialVolume = null,
            bool? isMuted = null,
            PresentationMotionMode? motionMode = null,
            bool? areOrbitGuidesEnabled = null,
            bool? areWorldLabelsEnabled = null,
            bool? hasCompletedOnboarding = null)
        {
            return new ExplorerSettingsSnapshot(
                masterVolume ?? Current.MasterVolume,
                musicVolume ?? Current.MusicVolume,
                uiVolume ?? Current.UiVolume,
                celestialVolume ?? Current.CelestialVolume,
                isMuted ?? Current.IsMuted,
                motionMode ?? Current.MotionMode,
                areOrbitGuidesEnabled ?? Current.AreOrbitGuidesEnabled,
                areWorldLabelsEnabled ?? Current.AreWorldLabelsEnabled,
                hasCompletedOnboarding ?? Current.HasCompletedOnboarding);
        }

        private void Apply(ExplorerSettingsSnapshot next)
        {
            if (Current == next)
            {
                return;
            }

            Current = next;
            store.Save(next);
            Changed?.Invoke();
        }
    }
}
