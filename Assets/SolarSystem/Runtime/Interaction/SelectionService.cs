using System;
using Tanvir.SolarSystem.Simulation;

namespace Tanvir.SolarSystem.Interaction
{
    /// <summary>Owns stable celestial selection state independently of camera focus.</summary>
    public sealed class SelectionService
    {
        /// <summary>Raised after the selected stable ID changes.</summary>
        public event Action<CelestialBodyId?> SelectionChanged;

        /// <summary>Gets the selected stable ID, or null when selection is empty.</summary>
        public CelestialBodyId? SelectedId { get; private set; }

        /// <summary>Selects a body and reports a real state change.</summary>
        public void Select(CelestialBodyId id)
        {
            if (SelectedId.HasValue && SelectedId.Value == id)
            {
                return;
            }

            SelectedId = id;
            SelectionChanged?.Invoke(SelectedId);
        }

        /// <summary>Clears selection and reports a real state change.</summary>
        public void Clear()
        {
            if (!SelectedId.HasValue)
            {
                return;
            }

            SelectedId = null;
            SelectionChanged?.Invoke(null);
        }
    }
}
