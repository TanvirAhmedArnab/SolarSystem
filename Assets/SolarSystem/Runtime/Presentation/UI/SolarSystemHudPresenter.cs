using System;
using System.Globalization;
using Tanvir.SolarSystem.Application;
using Tanvir.SolarSystem.Interaction;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
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
        private CelestialSelectionController selectionController;
        private UnityEngine.Camera explorerCamera;
        private VisualElement hudRoot;
        private VisualElement bodyInformationPanel;
        private VisualElement selectionReticle;
        private Label simulationState;
        private Label simulationRate;
        private Label selectionTarget;
        private Label pauseAction;
        private Label bodyName;
        private Label bodyCategory;
        private Label bodySummary;
        private Label bodyParent;
        private Label bodyRadius;
        private Label bodyMass;
        private Label bodyRotation;
        private Label bodyAxialTilt;
        private Label bodyOrbitDistance;
        private Label bodyOrbitPeriod;
        private Label bodySource;

        /// <summary>Gets whether the document and application state are connected.</summary>
        public bool IsInitialized { get; private set; }

        /// <summary>Gets the currently presented simulation-state text.</summary>
        public string SimulationStateText => simulationState?.text ?? string.Empty;

        /// <summary>Gets the currently presented time-rate text.</summary>
        public string SimulationRateText => simulationRate?.text ?? string.Empty;

        /// <summary>Gets the currently presented selection text.</summary>
        public string SelectionText => selectionTarget?.text ?? string.Empty;

        /// <summary>Gets whether the selected-body information card is visible.</summary>
        public bool IsBodyInformationVisible { get; private set; }

        /// <summary>Gets whether the screen-space selection reticle is visible.</summary>
        public bool IsSelectionReticleVisible { get; private set; }

        /// <summary>Gets the selected body's presented name.</summary>
        public string BodyNameText => bodyName?.text ?? string.Empty;

        /// <summary>Gets the selected body's presented radius.</summary>
        public string BodyRadiusText => bodyRadius?.text ?? string.Empty;

        /// <summary>Gets the selected body's source record shown in the information card.</summary>
        public string BodySourceText => bodySource?.text ?? string.Empty;

        /// <summary>Gets the contextual action shown beneath the Space keycap.</summary>
        public string PauseActionText => pauseAction?.text ?? string.Empty;

        /// <summary>Initializes the HUD against read-only application services.</summary>
        public void Initialize(
            SimulationTimeControlService simulationTimeControls,
            CelestialSelectionController celestialSelectionController,
            UnityEngine.Camera camera)
        {
            Release();
            timeControls = simulationTimeControls ??
                throw new ArgumentNullException(nameof(simulationTimeControls));
            selectionController = celestialSelectionController ??
                throw new ArgumentNullException(nameof(celestialSelectionController));
            selection = selectionController.Service ??
                throw new InvalidOperationException(
                    "Selection controller must be initialized before the HUD.");
            explorerCamera = camera != null
                ? camera
                : throw new ArgumentNullException(nameof(camera));

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

        private void LateUpdate()
        {
            RefreshSelectionReticle();
        }

        private void Refresh()
        {
            if (!IsInitialized)
            {
                return;
            }

            SimulationClockSnapshot snapshot = timeControls.Snapshot;
            simulationState.text =
                snapshot.IsPaused ? "SIMULATION / PAUSED" : "SIMULATION / RUNNING";
            simulationState.EnableInClassList("is-paused", snapshot.IsPaused);

            int multiplier = timeControls.CurrentMultiplier;
            string dayUnit = multiplier == 1 ? "day" : "days";
            simulationRate.text = string.Format(
                CultureInfo.InvariantCulture,
                "TIME RATE / {0:N0}x / {0:N0} {1} PER REAL SECOND",
                multiplier,
                dayUnit);
            selectionTarget.text = selection.SelectedId.HasValue
                ? $"TARGET / {selection.SelectedId.Value.Value.ToUpperInvariant()}"
                : "TARGET / NONE";
            pauseAction.text = snapshot.IsPaused ? "RESUME" : "PAUSE";
            RefreshBodyInformation();
        }

        private void OnSelectionChanged(CelestialBodyId? selectedId)
        {
            Refresh();
            RefreshSelectionReticle();
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
            selectionController = null;
            explorerCamera = null;
            IsInitialized = false;
            IsBodyInformationVisible = false;
            IsSelectionReticleVisible = false;
            hudRoot = null;
            bodyInformationPanel = null;
            selectionReticle = null;
            simulationState = null;
            simulationRate = null;
            selectionTarget = null;
            pauseAction = null;
            bodyName = null;
            bodyCategory = null;
            bodySummary = null;
            bodyParent = null;
            bodyRadius = null;
            bodyMass = null;
            bodyRotation = null;
            bodyAxialTilt = null;
            bodyOrbitDistance = null;
            bodyOrbitPeriod = null;
            bodySource = null;
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

            hudRoot = RequireElement(root, "hud-root");
            bodyInformationPanel = RequireElement(root, "body-information-panel");
            selectionReticle = RequireElement(root, "selection-reticle");
            simulationState = RequireLabel(root, "simulation-state");
            simulationRate = RequireLabel(root, "simulation-rate");
            selectionTarget = RequireLabel(root, "selection-target");
            pauseAction = RequireLabel(root, "pause-action");
            bodyName = RequireLabel(root, "body-name");
            bodyCategory = RequireLabel(root, "body-category");
            bodySummary = RequireLabel(root, "body-summary");
            bodyParent = RequireLabel(root, "body-parent");
            bodyRadius = RequireLabel(root, "body-radius");
            bodyMass = RequireLabel(root, "body-mass");
            bodyRotation = RequireLabel(root, "body-rotation");
            bodyAxialTilt = RequireLabel(root, "body-axial-tilt");
            bodyOrbitDistance = RequireLabel(root, "body-orbit-distance");
            bodyOrbitPeriod = RequireLabel(root, "body-orbit-period");
            bodySource = RequireLabel(root, "body-source");
            IsInitialized = true;
            Refresh();
        }

        private void RefreshBodyInformation()
        {
            CelestialBodyView selectedView = selectionController?.SelectedView;
            if (selectedView == null || selectedView.Definition == null)
            {
                SetBodyInformationVisible(false);
                return;
            }

            CelestialBodyInformation information =
                CelestialBodyInformation.From(selectedView.Definition);
            bodyName.text = information.DisplayName;
            bodyCategory.text = information.Category.ToUpperInvariant();
            bodySummary.text = information.Summary;
            bodyParent.text = information.Parent;
            bodyRadius.text = information.Radius;
            bodyMass.text = information.Mass;
            bodyRotation.text = information.Rotation;
            bodyAxialTilt.text = information.AxialTilt;
            bodyOrbitDistance.text = information.OrbitDistance;
            bodyOrbitPeriod.text = information.OrbitPeriod;
            bodySource.text = information.SourceRecord;
            SetBodyInformationVisible(true);
        }

        private void RefreshSelectionReticle()
        {
            if (!IsInitialized || hudRoot == null || selectionReticle == null)
            {
                return;
            }

            CelestialBodyView selectedView = selectionController?.SelectedView;
            if (selectedView == null || explorerCamera == null)
            {
                SetSelectionReticleVisible(false);
                return;
            }

            Vector3 viewportPosition =
                explorerCamera.WorldToViewportPoint(selectedView.transform.position);
            bool onScreen =
                viewportPosition.z > 0f &&
                viewportPosition.x >= 0f &&
                viewportPosition.x <= 1f &&
                viewportPosition.y >= 0f &&
                viewportPosition.y <= 1f;
            float panelWidth = hudRoot.resolvedStyle.width;
            float panelHeight = hudRoot.resolvedStyle.height;
            if (!onScreen || panelWidth <= 0f || panelHeight <= 0f)
            {
                SetSelectionReticleVisible(false);
                return;
            }

            Vector3 radiusViewport = explorerCamera.WorldToViewportPoint(
                selectedView.transform.position +
                explorerCamera.transform.up * selectedView.CurrentDisplayRadius);
            float projectedDiameter =
                Mathf.Abs(radiusViewport.y - viewportPosition.y) * panelHeight * 2f;
            float size = Mathf.Clamp(projectedDiameter + 24f, 38f, 180f);
            selectionReticle.style.left = viewportPosition.x * panelWidth - size * 0.5f;
            selectionReticle.style.top =
                (1f - viewportPosition.y) * panelHeight - size * 0.5f;
            selectionReticle.style.width = size;
            selectionReticle.style.height = size;
            SetSelectionReticleVisible(true);
        }

        private void SetBodyInformationVisible(bool visible)
        {
            IsBodyInformationVisible = visible;
            bodyInformationPanel.EnableInClassList("is-hidden", !visible);
        }

        private void SetSelectionReticleVisible(bool visible)
        {
            IsSelectionReticleVisible = visible;
            selectionReticle.EnableInClassList("is-hidden", !visible);
        }

        private static Label RequireLabel(VisualElement root, string name)
        {
            Label label = root.Q<Label>(name);
            return label != null
                ? label
                : throw new InvalidOperationException($"HUD is missing label '{name}'.");
        }

        private static VisualElement RequireElement(VisualElement root, string name)
        {
            VisualElement element = root.Q<VisualElement>(name);
            return element != null
                ? element
                : throw new InvalidOperationException($"HUD is missing element '{name}'.");
        }
    }
}
