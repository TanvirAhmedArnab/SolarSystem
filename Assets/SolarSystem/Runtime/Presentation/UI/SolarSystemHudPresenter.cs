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
        private GuidedScaleComparisonService scaleComparison;
        private SelectionService selection;
        private CelestialSelectionController selectionController;
        private UnityEngine.Camera explorerCamera;
        private VisualElement hudRoot;
        private VisualElement bodyInformationPanel;
        private VisualElement selectionReticle;
        private Label simulationState;
        private Label simulationRate;
        private Label selectionTarget;
        private Label scaleMode;
        private Label pauseAction;
        private VisualElement comparisonPanel;
        private Label comparisonProgress;
        private Label comparisonTitle;
        private Label comparisonMetric;
        private Label comparisonDescription;
        private Label comparisonNextAction;
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
        private Label bodyScaleNote;
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

        /// <summary>Gets the selected body's presentation-scale disclosure.</summary>
        public string BodyScaleNoteText => bodyScaleNote?.text ?? string.Empty;

        /// <summary>Gets the contextual action shown beneath the Space keycap.</summary>
        public string PauseActionText => pauseAction?.text ?? string.Empty;

        /// <summary>Gets the active scale-mode disclosure.</summary>
        public string ScaleModeText => scaleMode?.text ?? string.Empty;

        /// <summary>Gets whether the guided educational card is visible.</summary>
        public bool IsScaleComparisonVisible { get; private set; }

        /// <summary>Gets the guided comparison's current title.</summary>
        public string ScaleComparisonTitleText => comparisonTitle?.text ?? string.Empty;

        /// <summary>Gets the guided comparison's primary numeric explanation.</summary>
        public string ScaleComparisonMetricText => comparisonMetric?.text ?? string.Empty;

        /// <summary>Initializes the HUD against read-only application services.</summary>
        public void Initialize(
            SimulationTimeControlService simulationTimeControls,
            CelestialSelectionController celestialSelectionController,
            UnityEngine.Camera camera,
            GuidedScaleComparisonService guidedScaleComparison)
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
            scaleComparison = guidedScaleComparison ??
                throw new ArgumentNullException(nameof(guidedScaleComparison));

            if (document == null || styleSheet == null)
            {
                throw new InvalidOperationException(
                    "HUD presenter requires a UI document and style sheet.");
            }

            timeControls.Changed += Refresh;
            selection.SelectionChanged += OnSelectionChanged;
            scaleComparison.Changed += Refresh;
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
            string rotationUnit =
                multiplier == 1 ? "EARTH ROTATION" : "EARTH ROTATIONS";
            simulationRate.text = string.Format(
                CultureInfo.InvariantCulture,
                "TIME RATE / {0:N0}x / {0:N0} {1} PER REAL SECOND",
                multiplier,
                rotationUnit);
            selectionTarget.text = selection.SelectedId.HasValue
                ? $"TARGET / {selection.SelectedId.Value.Value.ToUpperInvariant()}"
                : "TARGET / NONE";
            scaleMode.text = scaleComparison.Stage switch
            {
                GuidedScaleComparisonStage.NormalizedOrbits =>
                    "SCALE / LINEAR ORBITS / 1 UNIT = 37.659 MILLION KM",
                GuidedScaleComparisonStage.LiteralEarthReference =>
                    "SCALE / LITERAL / EARTH RADIUS = 1",
                _ => "SCALE / READABLE OVERVIEW / ORBITS COMPRESSED"
            };
            pauseAction.text = snapshot.IsPaused ? "RESUME" : "PAUSE";
            RefreshScaleComparison();
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

            if (scaleComparison != null)
            {
                scaleComparison.Changed -= Refresh;
            }

            timeControls = null;
            selection = null;
            selectionController = null;
            explorerCamera = null;
            scaleComparison = null;
            IsInitialized = false;
            IsBodyInformationVisible = false;
            IsSelectionReticleVisible = false;
            IsScaleComparisonVisible = false;
            hudRoot = null;
            bodyInformationPanel = null;
            selectionReticle = null;
            comparisonPanel = null;
            simulationState = null;
            simulationRate = null;
            selectionTarget = null;
            scaleMode = null;
            pauseAction = null;
            comparisonProgress = null;
            comparisonTitle = null;
            comparisonMetric = null;
            comparisonDescription = null;
            comparisonNextAction = null;
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
            bodyScaleNote = null;
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
            comparisonPanel = RequireElement(root, "scale-comparison-panel");
            simulationState = RequireLabel(root, "simulation-state");
            simulationRate = RequireLabel(root, "simulation-rate");
            selectionTarget = RequireLabel(root, "selection-target");
            scaleMode = RequireLabel(root, "scale-mode");
            pauseAction = RequireLabel(root, "pause-action");
            comparisonProgress = RequireLabel(root, "comparison-progress");
            comparisonTitle = RequireLabel(root, "comparison-title");
            comparisonMetric = RequireLabel(root, "comparison-metric");
            comparisonDescription = RequireLabel(root, "comparison-description");
            comparisonNextAction = RequireLabel(root, "comparison-next-action");
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
            bodyScaleNote = RequireLabel(root, "body-scale-note");
            bodySource = RequireLabel(root, "body-source");
            IsInitialized = true;
            Refresh();
        }

        private void RefreshBodyInformation()
        {
            if (scaleComparison?.IsActive == true)
            {
                SetBodyInformationVisible(false);
                return;
            }

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
            bodyScaleNote.text = selectedView.StableId == "earth"
                ? "SCALE NOTE / EARTH'S SURFACE RADIUS IS PROPORTIONAL; " +
                  "CLOUD AND ATMOSPHERE THICKNESS ARE EXAGGERATED FOR READABILITY."
                : "SCALE NOTE / BODY RADII ARE EARTH-PROPORTIONAL; " +
                  "ORBIT SPACING IS COMPRESSED.";
            bodySource.text = information.SourceRecord;
            SetBodyInformationVisible(true);
        }

        private void RefreshSelectionReticle()
        {
            if (!IsInitialized || hudRoot == null || selectionReticle == null)
            {
                return;
            }

            if (scaleComparison?.IsActive == true)
            {
                SetSelectionReticleVisible(false);
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

        private void RefreshScaleComparison()
        {
            bool visible = scaleComparison.IsActive;
            IsScaleComparisonVisible = visible;
            comparisonPanel.EnableInClassList("is-hidden", !visible);
            hudRoot.EnableInClassList("comparison-active", visible);
            if (!visible)
            {
                return;
            }

            comparisonProgress.text =
                $"GUIDED SCALE COMPARISON / STEP {scaleComparison.CurrentStep} " +
                $"OF {scaleComparison.StepCount}";
            switch (scaleComparison.Stage)
            {
                case GuidedScaleComparisonStage.ReadableOverview:
                    comparisonTitle.text = "READABLE OVERVIEW";
                    comparisonMetric.text =
                        "BODY RADII: EARTH-PROPORTIONAL / ORBITS: LOGARITHMIC";
                    comparisonDescription.text =
                        "The whole system fits because empty orbital distance is " +
                        "compressed. Planet-to-planet size ratios remain honest.";
                    comparisonNextAction.text = "NEXT";
                    break;
                case GuidedScaleComparisonStage.NormalizedOrbits:
                    comparisonTitle.text = "LINEAR ORBIT SPACING";
                    comparisonMetric.text =
                        "1 ORBIT UNIT = 37.659 MILLION KM";
                    comparisonDescription.text =
                        "Sizes and distances now share one linear scale. Real bodies " +
                        "shrink below a pixel; the visible orbit lines are guides.";
                    comparisonNextAction.text = "NEXT";
                    break;
                case GuidedScaleComparisonStage.LiteralEarthReference:
                    comparisonTitle.text = "LITERAL EARTH-RADIUS REFERENCE";
                    comparisonMetric.text =
                        "EARTH RADIUS = 1 / EARTH-SUN ≈ 23,481";
                    comparisonDescription.text =
                        "Earth is effectively invisible beside the Sun, and Neptune " +
                        "lies far outside this Sun-Earth frame. This is the real scale problem.";
                    comparisonNextAction.text = "FINISH";
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Unsupported comparison stage '{scaleComparison.Stage}'.");
            }
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
