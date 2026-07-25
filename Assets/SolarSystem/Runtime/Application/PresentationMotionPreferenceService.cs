using System;

namespace Tanvir.SolarSystem.Application
{
    /// <summary>Identifies the user's preferred guided-presentation motion level.</summary>
    public enum PresentationMotionMode
    {
        FullMotion = 0,
        ReducedMotion = 1
    }

    /// <summary>Persists the presentation-motion preference outside the application core.</summary>
    public interface IPresentationMotionPreferenceStore
    {
        /// <summary>Attempts to load a previously saved preference.</summary>
        bool TryLoad(out PresentationMotionMode mode);

        /// <summary>Persists one validated preference.</summary>
        void Save(PresentationMotionMode mode);
    }

    /// <summary>
    /// Owns the persisted, allocation-stable presentation-motion preference.
    /// </summary>
    public sealed class PresentationMotionPreferenceService
    {
        private readonly IPresentationMotionPreferenceStore store;

        /// <summary>Creates the service and loads a valid saved value when available.</summary>
        public PresentationMotionPreferenceService(
            IPresentationMotionPreferenceStore preferenceStore,
            PresentationMotionMode defaultMode = PresentationMotionMode.FullMotion)
        {
            store = preferenceStore ??
                throw new ArgumentNullException(nameof(preferenceStore));
            Validate(defaultMode);
            Mode = store.TryLoad(out PresentationMotionMode savedMode) &&
                IsValid(savedMode)
                    ? savedMode
                    : defaultMode;
        }

        /// <summary>Raised after the effective preference changes.</summary>
        public event Action Changed;

        /// <summary>Gets the active presentation-motion preference.</summary>
        public PresentationMotionMode Mode { get; private set; }

        /// <summary>Gets whether guided transitions should complete instantly.</summary>
        public bool IsReducedMotion => Mode == PresentationMotionMode.ReducedMotion;

        /// <summary>Toggles between full and reduced motion.</summary>
        public void Toggle()
        {
            SetMode(IsReducedMotion
                ? PresentationMotionMode.FullMotion
                : PresentationMotionMode.ReducedMotion);
        }

        /// <summary>Applies and persists one validated preference.</summary>
        public void SetMode(PresentationMotionMode mode)
        {
            Validate(mode);
            if (Mode == mode)
            {
                return;
            }

            Mode = mode;
            store.Save(mode);
            Changed?.Invoke();
        }

        private static void Validate(PresentationMotionMode mode)
        {
            if (!IsValid(mode))
            {
                throw new ArgumentOutOfRangeException(nameof(mode));
            }
        }

        private static bool IsValid(PresentationMotionMode mode) =>
            mode == PresentationMotionMode.FullMotion ||
            mode == PresentationMotionMode.ReducedMotion;
    }
}
