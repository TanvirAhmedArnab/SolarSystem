using System;

namespace Tanvir.SolarSystem.Application
{
    /// <summary>Immutable player-facing explorer settings stored between sessions.</summary>
    public readonly struct ExplorerSettingsSnapshot : IEquatable<ExplorerSettingsSnapshot>
    {
        public const float DefaultMasterVolume = 0.65f;
        public const float DefaultMusicVolume = 0.18f;
        public const float DefaultUiVolume = 0.45f;
        public const float DefaultCelestialVolume = 0.22f;

        /// <summary>Creates a validated settings snapshot.</summary>
        public ExplorerSettingsSnapshot(
            float masterVolume,
            float musicVolume,
            float uiVolume,
            float celestialVolume,
            bool isMuted,
            PresentationMotionMode motionMode,
            bool areOrbitGuidesEnabled,
            bool areWorldLabelsEnabled,
            bool hasCompletedOnboarding)
        {
            ValidateVolume(masterVolume, nameof(masterVolume));
            ValidateVolume(musicVolume, nameof(musicVolume));
            ValidateVolume(uiVolume, nameof(uiVolume));
            ValidateVolume(celestialVolume, nameof(celestialVolume));
            if (motionMode != PresentationMotionMode.FullMotion &&
                motionMode != PresentationMotionMode.ReducedMotion)
            {
                throw new ArgumentOutOfRangeException(nameof(motionMode));
            }

            MasterVolume = masterVolume;
            MusicVolume = musicVolume;
            UiVolume = uiVolume;
            CelestialVolume = celestialVolume;
            IsMuted = isMuted;
            MotionMode = motionMode;
            AreOrbitGuidesEnabled = areOrbitGuidesEnabled;
            AreWorldLabelsEnabled = areWorldLabelsEnabled;
            HasCompletedOnboarding = hasCompletedOnboarding;
        }

        public float MasterVolume { get; }
        public float MusicVolume { get; }
        public float UiVolume { get; }
        public float CelestialVolume { get; }
        public bool IsMuted { get; }
        public PresentationMotionMode MotionMode { get; }
        public bool AreOrbitGuidesEnabled { get; }
        public bool AreWorldLabelsEnabled { get; }
        public bool HasCompletedOnboarding { get; }

        /// <summary>Creates the approved release defaults.</summary>
        public static ExplorerSettingsSnapshot CreateDefaults(
            bool hasCompletedOnboarding = false)
        {
            return new ExplorerSettingsSnapshot(
                DefaultMasterVolume,
                DefaultMusicVolume,
                DefaultUiVolume,
                DefaultCelestialVolume,
                false,
                PresentationMotionMode.FullMotion,
                true,
                true,
                hasCompletedOnboarding);
        }

        public bool Equals(ExplorerSettingsSnapshot other)
        {
            return MasterVolume.Equals(other.MasterVolume) &&
                MusicVolume.Equals(other.MusicVolume) &&
                UiVolume.Equals(other.UiVolume) &&
                CelestialVolume.Equals(other.CelestialVolume) &&
                IsMuted == other.IsMuted &&
                MotionMode == other.MotionMode &&
                AreOrbitGuidesEnabled == other.AreOrbitGuidesEnabled &&
                AreWorldLabelsEnabled == other.AreWorldLabelsEnabled &&
                HasCompletedOnboarding == other.HasCompletedOnboarding;
        }

        public override bool Equals(object obj) =>
            obj is ExplorerSettingsSnapshot other && Equals(other);

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(MasterVolume);
            hash.Add(MusicVolume);
            hash.Add(UiVolume);
            hash.Add(CelestialVolume);
            hash.Add(IsMuted);
            hash.Add(MotionMode);
            hash.Add(AreOrbitGuidesEnabled);
            hash.Add(AreWorldLabelsEnabled);
            hash.Add(HasCompletedOnboarding);
            return hash.ToHashCode();
        }

        public static bool operator ==(
            ExplorerSettingsSnapshot left,
            ExplorerSettingsSnapshot right) => left.Equals(right);

        public static bool operator !=(
            ExplorerSettingsSnapshot left,
            ExplorerSettingsSnapshot right) => !left.Equals(right);

        private static void ValidateVolume(float value, string parameterName)
        {
            if (!float.IsFinite(value) || value < 0f || value > 1f)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }
    }
}
