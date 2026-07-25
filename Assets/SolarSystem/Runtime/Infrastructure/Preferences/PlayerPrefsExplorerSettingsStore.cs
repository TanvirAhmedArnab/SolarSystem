using System;
using Tanvir.SolarSystem.Application;
using UnityEngine;

namespace Tanvir.SolarSystem.Infrastructure.Preferences
{
    /// <summary>Persists one versioned explorer settings document through PlayerPrefs.</summary>
    public sealed class PlayerPrefsExplorerSettingsStore : IExplorerSettingsStore
    {
        public const string PreferenceKey = "Tanvir.SolarSystem.ExplorerSettings.v1";
        private const int CurrentSchemaVersion = 1;

        [Serializable]
        private sealed class StoredSettings
        {
            public int schemaVersion;
            public float masterVolume;
            public float musicVolume;
            public float uiVolume;
            public float celestialVolume;
            public bool isMuted;
            public int motionMode;
            public bool areOrbitGuidesEnabled;
            public bool areWorldLabelsEnabled;
            public bool hasCompletedOnboarding;
        }

        public bool TryLoad(out ExplorerSettingsSnapshot settings)
        {
            settings = default;
            if (!PlayerPrefs.HasKey(PreferenceKey))
            {
                return false;
            }

            try
            {
                StoredSettings stored =
                    JsonUtility.FromJson<StoredSettings>(
                        PlayerPrefs.GetString(PreferenceKey));
                if (stored == null || stored.schemaVersion != CurrentSchemaVersion)
                {
                    return false;
                }

                settings = new ExplorerSettingsSnapshot(
                    stored.masterVolume,
                    stored.musicVolume,
                    stored.uiVolume,
                    stored.celestialVolume,
                    stored.isMuted,
                    (PresentationMotionMode)stored.motionMode,
                    stored.areOrbitGuidesEnabled,
                    stored.areWorldLabelsEnabled,
                    stored.hasCompletedOnboarding);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public void Save(ExplorerSettingsSnapshot settings)
        {
            var stored = new StoredSettings
            {
                schemaVersion = CurrentSchemaVersion,
                masterVolume = settings.MasterVolume,
                musicVolume = settings.MusicVolume,
                uiVolume = settings.UiVolume,
                celestialVolume = settings.CelestialVolume,
                isMuted = settings.IsMuted,
                motionMode = (int)settings.MotionMode,
                areOrbitGuidesEnabled = settings.AreOrbitGuidesEnabled,
                areWorldLabelsEnabled = settings.AreWorldLabelsEnabled,
                hasCompletedOnboarding = settings.HasCompletedOnboarding
            };
            PlayerPrefs.SetString(PreferenceKey, JsonUtility.ToJson(stored));
            PlayerPrefs.Save();
        }
    }
}
