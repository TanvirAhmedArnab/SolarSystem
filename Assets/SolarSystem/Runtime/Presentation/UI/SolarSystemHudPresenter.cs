using System;
using System.Globalization;
using Tanvir.SolarSystem.Application;
using Tanvir.SolarSystem.Interaction;
using Tanvir.SolarSystem.Simulation;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tanvir.SolarSystem.Presentation.UI
{
    /// <summary>Presents read-only explorer state through a runtime UI Toolkit document.</summary>
    [DisallowMultipleComponent]
    public sealed class SolarSystemHudPresenter : MonoBehaviour
    {
        [SerializeField] private UIDocument document;
        [SerializeField] private StyleSheet styleSheet;

        private SimulationTimeControlService timeControls;
        private SelectionService selection;
        private Label simulationState;
        private Label simulationRate;
        private Label selectionTarget;
        private Label controlHints;

        /// <summary>Gets whether the document and application state are connected.</summary>
        public bool IsInitialized { get; private set; }

        /// <summary>Gets the currently presented simulation-state text.</summary>
        public string SimulationStateText => simulationState?.text ?? string.Empty;

        /// <summary>Gets the currently presented time-rate text.</summary>
        public string SimulationRateText => simulationRate?.text ?? string.Empty;

        /// <summary>Gets the currently presented selection text.</summary>
        public string SelectionText => selectionTarget?.text ?? string.Empty;

        /// <summary>Initializes the HUD against read-only application services.</summary>
        public void Initialize(
            SimulationTimeControlService simulationTimeControls,
            SelectionService selectionService)
        {
            Release();
            timeControls = simulationTimeControls ??
                throw new ArgumentNullException(nameof(simulationTimeControls));
            selection = selectionService ??
                throw new ArgumentNullException(nameof(selectionService));

            if (document == null || styleSheet == null)
            {
                throw new InvalidOperationException(
                    "HUD presenter requires a UI document and style sheet.");
            }

            timeControls.Changed += Refresh;
            selection.SelectionChanged += OnSelectionChanged;
            TryConnectDocument();
        }

        private void OnEnable()
        {
            TryConnectDocument();
        }

        private void OnDestroy()
        {
            Release();
        }

        private void Refresh()
        {
            if (!IsInitialized)
            {
                return;
            }

            SimulationClockSnapshot snapshot = timeControls.Snapshot;
            simulationState.text =
                snapshot.IsPaused ? "SIMULATION  ·  PAUSED" : "SIMULATION  ·  RUNNING";
            simulationState.EnableInClassList("is-paused", snapshot.IsPaused);

            int multiplier = timeControls.CurrentMultiplier;
            string dayUnit = multiplier == 1 ? "day" : "days";
            simulationRate.text = string.Format(
                CultureInfo.InvariantCulture,
                "TIME RATE  ·  {0:N0}x  ·  {0:N0} {1} / REAL SECOND",
                multiplier,
                dayUnit);
            selectionTarget.text = selection.SelectedId.HasValue
                ? $"TARGET  ·  {selection.SelectedId.Value.Value.ToUpperInvariant()}"
                : "TARGET  ·  NONE";
            controlHints.text =
                "SPACE  PAUSE / RESUME     [  SLOWER     ]  FASTER     F  FOCUS";
        }

        private void OnSelectionChanged(CelestialBodyId? selectedId)
        {
            Refresh();
        }

        private void Release()
        {
            if (timeControls != null)
            {
                timeControls.Changed -= Refresh;
            }

            if (selection != null)
            {
                selection.SelectionChanged -= OnSelectionChanged;
            }

            timeControls = null;
            selection = null;
            IsInitialized = false;
            simulationState = null;
            simulationRate = null;
            selectionTarget = null;
            controlHints = null;
        }

        private void TryConnectDocument()
        {
            if (IsInitialized || timeControls == null || document == null)
            {
                return;
            }

            VisualElement root = document.rootVisualElement;
            if (root == null)
            {
                return;
            }

            if (!root.styleSheets.Contains(styleSheet))
            {
                root.styleSheets.Add(styleSheet);
            }

            simulationState = RequireLabel(root, "simulation-state");
            simulationRate = RequireLabel(root, "simulation-rate");
            selectionTarget = RequireLabel(root, "selection-target");
            controlHints = RequireLabel(root, "control-hints");
            IsInitialized = true;
            Refresh();
        }

        private static Label RequireLabel(VisualElement root, string name)
        {
            Label label = root.Q<Label>(name);
            return label != null
                ? label
                : throw new InvalidOperationException($"HUD is missing label '{name}'.");
        }
    }
}
