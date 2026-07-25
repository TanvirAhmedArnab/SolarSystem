using System;

namespace Tanvir.SolarSystem.Application
{
    /// <summary>Owns user-facing navigator and world-label visibility state.</summary>
    public sealed class CelestialNavigationService
    {
        /// <summary>Raised after either effective visibility setting changes.</summary>
        public event Action Changed;

        /// <summary>Gets whether the celestial navigator is open.</summary>
        public bool IsNavigatorVisible { get; private set; }

        /// <summary>Gets whether projected celestial labels are enabled.</summary>
        public bool AreWorldLabelsEnabled { get; private set; } = true;

        /// <summary>Toggles navigator visibility.</summary>
        public void ToggleNavigator()
        {
            SetNavigatorVisible(!IsNavigatorVisible);
        }

        /// <summary>Sets navigator visibility and reports only effective changes.</summary>
        public void SetNavigatorVisible(bool visible)
        {
            if (IsNavigatorVisible == visible)
            {
                return;
            }

            IsNavigatorVisible = visible;
            Changed?.Invoke();
        }

        /// <summary>Toggles projected world-label visibility.</summary>
        public void ToggleWorldLabels()
        {
            SetWorldLabelsEnabled(!AreWorldLabelsEnabled);
        }

        /// <summary>Sets world-label visibility and reports only effective changes.</summary>
        public void SetWorldLabelsEnabled(bool enabled)
        {
            if (AreWorldLabelsEnabled == enabled)
            {
                return;
            }

            AreWorldLabelsEnabled = enabled;
            Changed?.Invoke();
        }
    }
}
