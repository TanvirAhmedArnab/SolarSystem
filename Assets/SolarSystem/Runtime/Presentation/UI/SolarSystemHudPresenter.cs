using System;
using System.Globalization;
using Tanvir.SolarSystem.Application;
using Tanvir.SolarSystem.Interaction;
using Tanvir.SolarSystem.Presentation.Camera;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
using Tanvir.SolarSystem.Simulation;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tanvir.SolarSystem.Presentation.UI
{
    /// <summary>Presents read-only explorer state through a runtime UI Toolkit document.</summary>
    [DefaultExecutionOrder(100)]
    [DisallowMultipleComponent]
    public sealed class SolarSystemHudPresenter : MonoBehaviour
    {
        private const float WorldLabelWidth = 118f;
        private const float WorldLabelHeight = 28f;
        private const float WorldLabelGap = 6f;
        private const float WorldLabelEdgeMargin = 8f;
        private const float NavigatorStatusGap = 12f;
        private const float CompactWidthThreshold = 1500f;
        private const float CompactHeightThreshold = 820f;

        private sealed class CelestialUiEntry
        {
            public CelestialBodyView View;
            public Button NavigatorButton;
            public Label NavigatorName;
            public Label WorldLabel;
            public string NavigatorNameText;
            public string SelectedNavigatorNameText;
            public string WorldLabelText;
            public string SelectedWorldLabelText;
        }

        [SerializeField] private UIDocument document;
        [SerializeField] private StyleSheet styleSheet;

        private SimulationTimeControlService timeControls;
        private GuidedScaleComparisonService scaleComparison;
        private CinematicTourController tourController;
        private CinematicTourService cinematicTour;
        private PresentationMotionPreferenceService motionPreference;
        private ExplorerSettingsController settingsController;
        private ExplorerSettingsService settings;
        private ExplorerMenuController menuController;
        private ExplorerMenuService menu;
        private SelectionService selection;
        private CelestialSelectionController selectionController;
        private SolarSystemCameraController cameraController;
        private CelestialNavigationController navigationController;
        private CelestialNavigationService navigation;
        private UnityEngine.Camera explorerCamera;
        private VisualElement hudRoot;
        private VisualElement statusPanel;
        private VisualElement hintPanel;
        private VisualElement bodyInformationPanel;
        private VisualElement selectionReticle;
        private VisualElement worldLabelLayer;
        private VisualElement navigatorPanel;
        private ScrollView navigatorList;
        private Label simulationState;
        private Label simulationRate;
        private Label selectionTarget;
        private Label scaleMode;
        private Label labelsState;
        private Label orbitState;
        private Label motionState;
        private Label pauseAction;
        private Button menuButton;
        private VisualElement menuOverlay;
        private Label menuTitle;
        private Button menuCloseButton;
        private Button menuHelpTab;
        private Button menuSettingsTab;
        private Button menuCreditsTab;
        private ScrollView helpPage;
        private ScrollView settingsPage;
        private ScrollView creditsPage;
        private Label onboardingLabel;
        private Slider masterVolumeSlider;
        private Slider musicVolumeSlider;
        private Slider uiVolumeSlider;
        private Slider celestialVolumeSlider;
        private Label masterVolumeValue;
        private Label musicVolumeValue;
        private Label uiVolumeValue;
        private Label celestialVolumeValue;
        private Toggle muteToggle;
        private Toggle reducedMotionToggle;
        private Toggle orbitGuidesToggle;
        private Toggle worldLabelsToggle;
        private Button restoreDefaultsButton;
        private VisualElement comparisonPanel;
        private Label comparisonProgress;
        private Label comparisonTitle;
        private Label comparisonMetric;
        private Label comparisonDescription;
        private Label comparisonNextAction;
        private VisualElement tourPanel;
        private Label tourProgress;
        private Label tourTitle;
        private Label tourSubtitle;
        private Label tourDescription;
        private Button tourNextButton;
        private Button tourMotionButton;
        private Button tourExitButton;
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
        private CelestialUiEntry[] celestialEntries = Array.Empty<CelestialUiEntry>();
        private Rect[] occupiedWorldLabelRects = Array.Empty<Rect>();
        private int visibleWorldLabelCount;
        private bool isCompactLayout;

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

        /// <summary>Gets the selection reticle's latest panel-space bounds.</summary>
        public Rect SelectionReticleWorldBound =>
            selectionReticle?.worldBound ?? Rect.zero;

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

        /// <summary>Gets whether the cinematic-tour chapter card is visible.</summary>
        public bool IsCinematicTourVisible { get; private set; }

        /// <summary>Gets the current cinematic-tour chapter title.</summary>
        public string CinematicTourTitleText => tourTitle?.text ?? string.Empty;

        /// <summary>Gets the motion preference presented on the tour control.</summary>
        public string CinematicTourMotionText =>
            tourMotionButton?.text ?? string.Empty;

        /// <summary>Gets whether the celestial navigator is currently visible.</summary>
        public bool IsNavigatorVisible => navigation?.IsNavigatorVisible == true;

        /// <summary>Gets whether the user preference enables projected labels.</summary>
        public bool AreWorldLabelsEnabled =>
            navigation?.AreWorldLabelsEnabled == true;

        /// <summary>Gets the number of deterministic navigator entries.</summary>
        public int NavigatorEntryCount => celestialEntries.Length;

        /// <summary>Gets the number of cached projected-label elements.</summary>
        public int WorldLabelCount => celestialEntries.Length;

        /// <summary>Gets the number of labels accepted by the latest overlap pass.</summary>
        public int VisibleWorldLabelCount => visibleWorldLabelCount;

        /// <summary>Gets the current label-state disclosure.</summary>
        public string LabelsStateText => labelsState?.text ?? string.Empty;

        /// <summary>Gets the current orbit-guide preference disclosure.</summary>
        public string OrbitStateText => orbitState?.text ?? string.Empty;

        /// <summary>Gets the current motion-accessibility disclosure.</summary>
        public string MotionStateText => motionState?.text ?? string.Empty;

        /// <summary>Gets whether the unified Explorer Menu is visible.</summary>
        public bool IsExplorerMenuVisible => menu?.IsOpen == true;

        /// <summary>Gets the active Explorer Menu page.</summary>
        public ExplorerMenuPage ActiveMenuPage =>
            menu?.ActivePage ?? ExplorerMenuPage.Help;

        /// <summary>Gets the currently presented master-volume value.</summary>
        public float MasterVolumeValue => masterVolumeSlider?.value ?? 0f;

        /// <summary>Gets the active HUD root bounds for responsive validation.</summary>
        public Rect HudWorldBound => hudRoot?.worldBound ?? Rect.zero;

        /// <summary>Gets the status card bounds for responsive validation.</summary>
        public Rect StatusPanelWorldBound => statusPanel?.worldBound ?? Rect.zero;

        /// <summary>Gets the navigator bounds for responsive validation.</summary>
        public Rect NavigatorWorldBound => navigatorPanel?.worldBound ?? Rect.zero;

        /// <summary>Gets one parent-first navigator stable ID.</summary>
        public string GetNavigatorEntryId(int index)
        {
            if (index < 0 || index >= celestialEntries.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return celestialEntries[index].View.StableId;
        }

        /// <summary>Gets whether one body's projected label survived the current pass.</summary>
        public bool IsWorldLabelVisible(string stableId)
        {
            foreach (CelestialUiEntry entry in celestialEntries)
            {
                if (entry.View.StableId == stableId)
                {
                    return !entry.WorldLabel.ClassListContains("is-hidden");
                }
            }

            return false;
        }

        /// <summary>Routes a navigator activation through the application controller.</summary>
        public bool NavigateTo(string stableId)
        {
            return navigationController != null &&
                navigationController.NavigateTo(stableId);
        }

        /// <summary>Sets navigator visibility for UI and regression validation.</summary>
        public void SetNavigatorVisible(bool visible)
        {
            navigationController?.SetNavigatorVisible(visible);
        }

        /// <summary>Sets projected-label visibility for UI and regression validation.</summary>
        public void SetWorldLabelsEnabled(bool enabled)
        {
            navigationController?.SetWorldLabelsEnabled(enabled);
        }

        /// <summary>Initializes the HUD against read-only application services.</summary>
        public void Initialize(
            SimulationTimeControlService simulationTimeControls,
            CelestialSelectionController celestialSelectionController,
            UnityEngine.Camera camera,
            GuidedScaleComparisonService guidedScaleComparison,
            SolarSystemCameraController explorerCameraController,
            CelestialNavigationController celestialNavigationController,
            CinematicTourController cinematicTourController,
            ExplorerSettingsController explorerSettingsController,
            ExplorerMenuController explorerMenuController)
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
            cameraController = explorerCameraController != null
                ? explorerCameraController
                : throw new ArgumentNullException(nameof(explorerCameraController));
            navigationController = celestialNavigationController != null
                ? celestialNavigationController
                : throw new ArgumentNullException(nameof(celestialNavigationController));
            navigation = navigationController.Service ??
                throw new InvalidOperationException(
                    "Navigation controller must be initialized before the HUD.");
            tourController = cinematicTourController ??
                throw new ArgumentNullException(nameof(cinematicTourController));
            cinematicTour = tourController.Service ??
                throw new InvalidOperationException(
                    "Cinematic tour controller must be initialized before the HUD.");
            motionPreference = tourController.MotionPreference ??
                throw new InvalidOperationException(
                    "Motion preference must be initialized before the HUD.");
            settingsController = explorerSettingsController ??
                throw new ArgumentNullException(nameof(explorerSettingsController));
            settings = settingsController.Service ??
                throw new InvalidOperationException(
                    "Explorer settings must be initialized before the HUD.");
            menuController = explorerMenuController ??
                throw new ArgumentNullException(nameof(explorerMenuController));
            menu = menuController.Service ??
                throw new InvalidOperationException(
                    "Explorer menu must be initialized before the HUD.");

            if (document == null || styleSheet == null)
            {
                throw new InvalidOperationException(
                    "HUD presenter requires a UI document and style sheet.");
            }

            timeControls.Changed += Refresh;
            selection.SelectionChanged += OnSelectionChanged;
            scaleComparison.Changed += Refresh;
            cinematicTour.Changed += Refresh;
            motionPreference.Changed += Refresh;
            navigation.Changed += OnNavigationChanged;
            settings.Changed += OnSettingsChanged;
            menu.Changed += OnMenuChanged;
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
            RefreshResponsiveState();
            RefreshSelectionReticle();
            RefreshWorldLabels();
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
            labelsState.text = navigation.AreWorldLabelsEnabled
                ? "LABELS / ON / L TO TOGGLE"
                : "LABELS / OFF / L TO TOGGLE";
            orbitState.text = settings.Current.AreOrbitGuidesEnabled
                ? "ORBITS / ON / O TO TOGGLE"
                : "ORBITS / OFF / O TO TOGGLE";
            motionState.text =
                settings.Current.MotionMode == PresentationMotionMode.ReducedMotion
                    ? "MOTION / REDUCED / M TO TOGGLE"
                    : "MOTION / FULL / M TO TOGGLE";
            RefreshScaleComparison();
            RefreshCinematicTour();
            RefreshBodyInformation();
            RefreshNavigator();
            RefreshSettings();
            RefreshMenu();
        }

        private void OnSelectionChanged(CelestialBodyId? selectedId)
        {
            Refresh();
            RefreshSelectionReticle();
            RefreshWorldLabels();
        }

        private void OnNavigationChanged()
        {
            if (!IsInitialized)
            {
                return;
            }

            Refresh();
            if (navigation.IsNavigatorVisible)
            {
                FocusSelectedNavigatorEntry();
            }

            RefreshWorldLabels();
        }

        private void OnSettingsChanged()
        {
            Refresh();
        }

        private void OnMenuChanged()
        {
            RefreshMenu();
        }

        private void Release()
        {
            statusPanel?.UnregisterCallback<GeometryChangedEvent>(
                OnStatusPanelGeometryChanged);
            navigatorPanel?.UnregisterCallback<KeyDownEvent>(
                OnNavigatorKeyDown,
                TrickleDown.TrickleDown);

            if (tourNextButton != null)
            {
                tourNextButton.clicked -= OnTourNextClicked;
            }

            if (tourExitButton != null)
            {
                tourExitButton.clicked -= OnTourExitClicked;
            }

            if (tourMotionButton != null)
            {
                tourMotionButton.clicked -= OnTourMotionClicked;
            }

            if (menuButton != null)
            {
                menuButton.clicked -= OnMenuButtonClicked;
            }

            if (menuCloseButton != null)
            {
                menuCloseButton.clicked -= OnMenuCloseClicked;
            }

            if (menuHelpTab != null)
            {
                menuHelpTab.clicked -= OnHelpTabClicked;
            }

            if (menuSettingsTab != null)
            {
                menuSettingsTab.clicked -= OnSettingsTabClicked;
            }

            if (menuCreditsTab != null)
            {
                menuCreditsTab.clicked -= OnCreditsTabClicked;
            }

            if (restoreDefaultsButton != null)
            {
                restoreDefaultsButton.clicked -= OnRestoreDefaultsClicked;
            }

            masterVolumeSlider?.UnregisterValueChangedCallback(OnMasterVolumeChanged);
            musicVolumeSlider?.UnregisterValueChangedCallback(OnMusicVolumeChanged);
            uiVolumeSlider?.UnregisterValueChangedCallback(OnUiVolumeChanged);
            celestialVolumeSlider?.UnregisterValueChangedCallback(
                OnCelestialVolumeChanged);
            muteToggle?.UnregisterValueChangedCallback(OnMuteChanged);
            reducedMotionToggle?.UnregisterValueChangedCallback(
                OnReducedMotionChanged);
            orbitGuidesToggle?.UnregisterValueChangedCallback(
                OnOrbitGuidesChanged);
            worldLabelsToggle?.UnregisterValueChangedCallback(
                OnWorldLabelsChanged);

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

            if (cinematicTour != null)
            {
                cinematicTour.Changed -= Refresh;
            }

            if (motionPreference != null)
            {
                motionPreference.Changed -= Refresh;
            }

            if (navigation != null)
            {
                navigation.Changed -= OnNavigationChanged;
            }

            if (settings != null)
            {
                settings.Changed -= OnSettingsChanged;
            }

            if (menu != null)
            {
                menu.Changed -= OnMenuChanged;
            }

            timeControls = null;
            selection = null;
            selectionController = null;
            cameraController = null;
            navigationController = null;
            navigation = null;
            explorerCamera = null;
            scaleComparison = null;
            tourController = null;
            cinematicTour = null;
            motionPreference = null;
            settingsController = null;
            settings = null;
            menuController = null;
            menu = null;
            IsInitialized = false;
            IsBodyInformationVisible = false;
            IsSelectionReticleVisible = false;
            IsScaleComparisonVisible = false;
            IsCinematicTourVisible = false;
            hudRoot = null;
            statusPanel = null;
            hintPanel = null;
            bodyInformationPanel = null;
            selectionReticle = null;
            worldLabelLayer = null;
            navigatorPanel = null;
            navigatorList = null;
            comparisonPanel = null;
            tourPanel = null;
            simulationState = null;
            simulationRate = null;
            selectionTarget = null;
            scaleMode = null;
            labelsState = null;
            orbitState = null;
            motionState = null;
            pauseAction = null;
            menuButton = null;
            menuOverlay = null;
            menuTitle = null;
            menuCloseButton = null;
            menuHelpTab = null;
            menuSettingsTab = null;
            menuCreditsTab = null;
            helpPage = null;
            settingsPage = null;
            creditsPage = null;
            onboardingLabel = null;
            masterVolumeSlider = null;
            musicVolumeSlider = null;
            uiVolumeSlider = null;
            celestialVolumeSlider = null;
            masterVolumeValue = null;
            musicVolumeValue = null;
            uiVolumeValue = null;
            celestialVolumeValue = null;
            muteToggle = null;
            reducedMotionToggle = null;
            orbitGuidesToggle = null;
            worldLabelsToggle = null;
            restoreDefaultsButton = null;
            comparisonProgress = null;
            comparisonTitle = null;
            comparisonMetric = null;
            comparisonDescription = null;
            comparisonNextAction = null;
            tourProgress = null;
            tourTitle = null;
            tourSubtitle = null;
            tourDescription = null;
            tourNextButton = null;
            tourMotionButton = null;
            tourExitButton = null;
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
            celestialEntries = Array.Empty<CelestialUiEntry>();
            occupiedWorldLabelRects = Array.Empty<Rect>();
            visibleWorldLabelCount = 0;
            isCompactLayout = false;
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
            statusPanel = RequireElement(root, "status-panel");
            hintPanel = RequireElement(root, "hint-panel");
            bodyInformationPanel = RequireElement(root, "body-information-panel");
            selectionReticle = RequireElement(root, "selection-reticle");
            worldLabelLayer = RequireElement(root, "world-label-layer");
            navigatorPanel = RequireElement(root, "navigator-panel");
            statusPanel.RegisterCallback<GeometryChangedEvent>(
                OnStatusPanelGeometryChanged);
            navigatorPanel.RegisterCallback<KeyDownEvent>(
                OnNavigatorKeyDown,
                TrickleDown.TrickleDown);
            navigatorList = RequireScrollView(root, "navigator-list");
            comparisonPanel = RequireElement(root, "scale-comparison-panel");
            tourPanel = RequireElement(root, "cinematic-tour-panel");
            simulationState = RequireLabel(root, "simulation-state");
            simulationRate = RequireLabel(root, "simulation-rate");
            selectionTarget = RequireLabel(root, "selection-target");
            scaleMode = RequireLabel(root, "scale-mode");
            labelsState = RequireLabel(root, "labels-state");
            orbitState = RequireLabel(root, "orbit-state");
            motionState = RequireLabel(root, "motion-state");
            pauseAction = RequireLabel(root, "pause-action");
            comparisonProgress = RequireLabel(root, "comparison-progress");
            comparisonTitle = RequireLabel(root, "comparison-title");
            comparisonMetric = RequireLabel(root, "comparison-metric");
            comparisonDescription = RequireLabel(root, "comparison-description");
            comparisonNextAction = RequireLabel(root, "comparison-next-action");
            tourProgress = RequireLabel(root, "tour-progress");
            tourTitle = RequireLabel(root, "tour-title");
            tourSubtitle = RequireLabel(root, "tour-subtitle");
            tourDescription = RequireLabel(root, "tour-description");
            tourNextButton = RequireButton(root, "tour-next-button");
            tourMotionButton = RequireButton(root, "tour-motion-button");
            tourExitButton = RequireButton(root, "tour-exit-button");
            tourNextButton.clicked += OnTourNextClicked;
            tourMotionButton.clicked += OnTourMotionClicked;
            tourExitButton.clicked += OnTourExitClicked;
            menuButton = RequireButton(root, "menu-button");
            menuOverlay = RequireElement(root, "menu-overlay");
            menuTitle = RequireLabel(root, "menu-title");
            menuCloseButton = RequireButton(root, "menu-close-button");
            menuHelpTab = RequireButton(root, "menu-help-tab");
            menuSettingsTab = RequireButton(root, "menu-settings-tab");
            menuCreditsTab = RequireButton(root, "menu-credits-tab");
            helpPage = RequireScrollView(root, "help-page");
            settingsPage = RequireScrollView(root, "settings-page");
            creditsPage = RequireScrollView(root, "credits-page");
            onboardingLabel = RequireLabel(root, "onboarding-label");
            masterVolumeSlider = RequireSlider(root, "master-volume-slider");
            musicVolumeSlider = RequireSlider(root, "music-volume-slider");
            uiVolumeSlider = RequireSlider(root, "ui-volume-slider");
            celestialVolumeSlider =
                RequireSlider(root, "celestial-volume-slider");
            masterVolumeValue = RequireLabel(root, "master-volume-value");
            musicVolumeValue = RequireLabel(root, "music-volume-value");
            uiVolumeValue = RequireLabel(root, "ui-volume-value");
            celestialVolumeValue =
                RequireLabel(root, "celestial-volume-value");
            muteToggle = RequireToggle(root, "mute-toggle");
            reducedMotionToggle = RequireToggle(root, "reduced-motion-toggle");
            orbitGuidesToggle = RequireToggle(root, "orbit-guides-toggle");
            worldLabelsToggle = RequireToggle(root, "world-labels-toggle");
            restoreDefaultsButton =
                RequireButton(root, "restore-defaults-button");
            menuButton.clicked += OnMenuButtonClicked;
            menuCloseButton.clicked += OnMenuCloseClicked;
            menuHelpTab.clicked += OnHelpTabClicked;
            menuSettingsTab.clicked += OnSettingsTabClicked;
            menuCreditsTab.clicked += OnCreditsTabClicked;
            restoreDefaultsButton.clicked += OnRestoreDefaultsClicked;
            masterVolumeSlider.RegisterValueChangedCallback(OnMasterVolumeChanged);
            musicVolumeSlider.RegisterValueChangedCallback(OnMusicVolumeChanged);
            uiVolumeSlider.RegisterValueChangedCallback(OnUiVolumeChanged);
            celestialVolumeSlider.RegisterValueChangedCallback(
                OnCelestialVolumeChanged);
            muteToggle.RegisterValueChangedCallback(OnMuteChanged);
            reducedMotionToggle.RegisterValueChangedCallback(
                OnReducedMotionChanged);
            orbitGuidesToggle.RegisterValueChangedCallback(
                OnOrbitGuidesChanged);
            worldLabelsToggle.RegisterValueChangedCallback(
                OnWorldLabelsChanged);
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
            BuildCelestialUi();
            IsInitialized = true;
            Refresh();
        }

        private void BuildCelestialUi()
        {
            navigatorList.Clear();
            worldLabelLayer.Clear();
            int count = navigationController.OrderedViews.Count;
            celestialEntries = new CelestialUiEntry[count];
            occupiedWorldLabelRects = new Rect[count];

            for (int index = 0; index < count; index++)
            {
                CelestialBodyView view = navigationController.OrderedViews[index];
                string displayName = view.Definition.DisplayName.ToUpperInvariant();
                var button = new Button
                {
                    name = $"navigator-entry-{view.StableId}",
                    userData = view.StableId,
                    focusable = true,
                    tabIndex = index
                };
                button.AddToClassList("navigator-row");
                if (view.Definition.Category == CelestialBodyCategory.Moon)
                {
                    button.AddToClassList("is-moon");
                }

                var nameLabel = new Label(displayName);
                nameLabel.AddToClassList("navigator-row-name");
                var typeLabel = new Label(BuildNavigatorType(view));
                typeLabel.AddToClassList("navigator-row-type");
                button.Add(nameLabel);
                button.Add(typeLabel);
                button.RegisterCallback<ClickEvent>(OnNavigatorEntryClicked);
                navigatorList.Add(button);

                var worldLabel = new Label(displayName)
                {
                    name = $"world-label-{view.StableId}",
                    pickingMode = PickingMode.Ignore
                };
                worldLabel.AddToClassList("world-label");
                worldLabel.AddToClassList("is-hidden");
                worldLabelLayer.Add(worldLabel);

                celestialEntries[index] = new CelestialUiEntry
                {
                    View = view,
                    NavigatorButton = button,
                    NavigatorName = nameLabel,
                    WorldLabel = worldLabel,
                    NavigatorNameText = displayName,
                    SelectedNavigatorNameText = $"› {displayName}",
                    WorldLabelText = displayName,
                    SelectedWorldLabelText = $"[ {displayName} ]"
                };
            }
        }

        private void OnStatusPanelGeometryChanged(GeometryChangedEvent evt)
        {
            if (navigatorPanel == null || evt.newRect.height <= 0f)
            {
                return;
            }

            navigatorPanel.style.top = evt.newRect.yMax + NavigatorStatusGap;
        }

        private void OnNavigatorKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode != KeyCode.Return &&
                evt.keyCode != KeyCode.KeypadEnter)
            {
                return;
            }

            if (TryNavigateFromNavigatorElement(evt.target as VisualElement))
            {
                evt.PreventDefault();
                evt.StopImmediatePropagation();
            }
        }

        private string BuildNavigatorType(CelestialBodyView view)
        {
            if (view.Definition.Category != CelestialBodyCategory.Moon)
            {
                return view.Definition.Category.ToString().ToUpperInvariant();
            }

            string parentId = view.Definition.ParentId;
            foreach (CelestialBodyView candidate in navigationController.OrderedViews)
            {
                if (candidate.StableId == parentId)
                {
                    return $"MOON / {candidate.Definition.DisplayName.ToUpperInvariant()}";
                }
            }

            return "MOON";
        }

        private void OnNavigatorEntryClicked(ClickEvent evt)
        {
            TryNavigateFromNavigatorElement(evt.currentTarget as VisualElement);
        }

        private bool TryNavigateFromNavigatorElement(VisualElement element)
        {
            VisualElement candidate = element;
            while (candidate != null && candidate != navigatorPanel)
            {
                if (candidate.userData is string stableId)
                {
                    return navigationController.NavigateTo(stableId);
                }

                candidate = candidate.parent;
            }

            return false;
        }

        private void RefreshNavigator()
        {
            bool visible = navigation.IsNavigatorVisible &&
                !scaleComparison.IsActive &&
                cinematicTour?.IsActive != true;
            navigatorPanel.EnableInClassList("is-hidden", !visible);
            CelestialBodyView selected = selectionController.SelectedView;
            foreach (CelestialUiEntry entry in celestialEntries)
            {
                bool isSelected = entry.View == selected;
                entry.NavigatorButton.EnableInClassList("is-selected", isSelected);
                entry.NavigatorName.text = isSelected
                    ? entry.SelectedNavigatorNameText
                    : entry.NavigatorNameText;
                entry.WorldLabel.EnableInClassList("is-selected", isSelected);
                entry.WorldLabel.text = isSelected
                    ? entry.SelectedWorldLabelText
                    : entry.WorldLabelText;
            }
        }

        private void FocusSelectedNavigatorEntry()
        {
            CelestialBodyView selected = selectionController.SelectedView;
            foreach (CelestialUiEntry entry in celestialEntries)
            {
                if (entry.View == selected)
                {
                    entry.NavigatorButton.Focus();
                    return;
                }
            }

            if (celestialEntries.Length > 0)
            {
                celestialEntries[0].NavigatorButton.Focus();
            }
        }

        private void RefreshResponsiveState()
        {
            if (!IsInitialized || hudRoot == null)
            {
                return;
            }

            float width = hudRoot.resolvedStyle.width;
            float height = hudRoot.resolvedStyle.height;
            bool compact =
                width > 0f &&
                height > 0f &&
                (width < CompactWidthThreshold || height < CompactHeightThreshold);
            if (compact == isCompactLayout)
            {
                return;
            }

            isCompactLayout = compact;
            hudRoot.EnableInClassList("is-compact", compact);
        }

        private void RefreshWorldLabels()
        {
            if (!IsInitialized ||
                worldLabelLayer == null ||
                !navigation.AreWorldLabelsEnabled ||
                scaleComparison.IsActive ||
                cinematicTour?.IsActive == true)
            {
                HideAllWorldLabels();
                return;
            }

            float width = hudRoot.resolvedStyle.width;
            float height = hudRoot.resolvedStyle.height;
            if (width <= 0f || height <= 0f)
            {
                HideAllWorldLabels();
                return;
            }

            HideAllWorldLabels();
            int occupiedCount = 0;
            CelestialBodyView selected = selectionController.SelectedView;
            CelestialBodyView focused = cameraController.FocusedTarget;

            if (cameraController.Mode != SolarSystemCameraMode.FreeFlight)
            {
                CelestialBodyView priority = focused != null ? focused : selected;
                if (priority != null)
                {
                    TryShowWorldLabel(priority, width, height, ref occupiedCount);
                }

                visibleWorldLabelCount = occupiedCount;
                return;
            }

            if (selected != null)
            {
                TryShowWorldLabel(selected, width, height, ref occupiedCount);
            }

            foreach (CelestialUiEntry entry in celestialEntries)
            {
                if (entry.View == selected ||
                    entry.View.Definition.Category == CelestialBodyCategory.Moon)
                {
                    continue;
                }

                TryShowWorldLabel(entry.View, width, height, ref occupiedCount);
            }

            foreach (CelestialUiEntry entry in celestialEntries)
            {
                if (entry.View == selected ||
                    entry.View.Definition.Category != CelestialBodyCategory.Moon)
                {
                    continue;
                }

                TryShowWorldLabel(entry.View, width, height, ref occupiedCount);
            }

            visibleWorldLabelCount = occupiedCount;
        }

        private void TryShowWorldLabel(
            CelestialBodyView view,
            float panelWidth,
            float panelHeight,
            ref int occupiedCount)
        {
            CelestialUiEntry entry = FindEntry(view);
            if (entry == null)
            {
                return;
            }

            Vector3 centerViewport =
                explorerCamera.WorldToViewportPoint(view.transform.position);
            if (centerViewport.z <= 0f ||
                centerViewport.x <= 0.01f ||
                centerViewport.x >= 0.99f ||
                centerViewport.y <= 0.01f ||
                centerViewport.y >= 0.99f)
            {
                return;
            }

            Vector3 topViewport = explorerCamera.WorldToViewportPoint(
                view.transform.position +
                explorerCamera.transform.up * view.CurrentDisplayRadius);
            float left =
                centerViewport.x * panelWidth - (WorldLabelWidth * 0.5f);
            float aboveTop =
                (1f - Mathf.Max(centerViewport.y, topViewport.y)) * panelHeight -
                WorldLabelHeight -
                WorldLabelGap;
            Rect candidate = new Rect(
                left,
                aboveTop,
                WorldLabelWidth,
                WorldLabelHeight);
            if (!CanPlaceWorldLabel(candidate, panelWidth, panelHeight, occupiedCount))
            {
                float belowTop =
                    (1f - Mathf.Min(centerViewport.y, topViewport.y)) * panelHeight +
                    WorldLabelGap;
                candidate.y = belowTop;
                if (!CanPlaceWorldLabel(
                        candidate,
                        panelWidth,
                        panelHeight,
                        occupiedCount))
                {
                    return;
                }
            }

            entry.WorldLabel.style.left = candidate.x;
            entry.WorldLabel.style.top = candidate.y;
            entry.WorldLabel.EnableInClassList("is-hidden", false);
            occupiedWorldLabelRects[occupiedCount] = candidate;
            occupiedCount++;
        }

        private bool CanPlaceWorldLabel(
            Rect candidate,
            float panelWidth,
            float panelHeight,
            int occupiedCount)
        {
            if (candidate.xMin < WorldLabelEdgeMargin ||
                candidate.yMin < WorldLabelEdgeMargin ||
                candidate.xMax > panelWidth - WorldLabelEdgeMargin ||
                candidate.yMax > panelHeight - WorldLabelEdgeMargin ||
                OverlapsPanel(candidate, statusPanel, true) ||
                OverlapsPanel(candidate, hintPanel, true) ||
                OverlapsPanel(candidate, bodyInformationPanel, IsBodyInformationVisible) ||
                OverlapsPanel(candidate, navigatorPanel, navigation.IsNavigatorVisible))
            {
                return false;
            }

            for (int index = 0; index < occupiedCount; index++)
            {
                if (candidate.Overlaps(occupiedWorldLabelRects[index]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool OverlapsPanel(
            Rect candidate,
            VisualElement panel,
            bool include)
        {
            if (!include || panel == null)
            {
                return false;
            }

            Rect panelBounds = panel.worldBound;
            if (panelBounds.width <= 0f || panelBounds.height <= 0f)
            {
                return false;
            }

            Rect hudBounds = hudRoot.worldBound;
            panelBounds.position -= hudBounds.position;
            return candidate.Overlaps(panelBounds);
        }

        private CelestialUiEntry FindEntry(CelestialBodyView view)
        {
            foreach (CelestialUiEntry entry in celestialEntries)
            {
                if (entry.View == view)
                {
                    return entry;
                }
            }

            return null;
        }

        private void HideAllWorldLabels()
        {
            visibleWorldLabelCount = 0;
            foreach (CelestialUiEntry entry in celestialEntries)
            {
                entry.WorldLabel.EnableInClassList("is-hidden", true);
            }
        }

        private void RefreshBodyInformation()
        {
            if (scaleComparison?.IsActive == true ||
                cinematicTour?.IsActive == true)
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

            if (scaleComparison?.IsActive == true ||
                cinematicTour?.IsActive == true)
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
            bool isFocusedSelection =
                cameraController != null &&
                cameraController.FocusedTarget == selectedView &&
                (cameraController.Mode == SolarSystemCameraMode.FocusTransition ||
                 cameraController.Mode == SolarSystemCameraMode.Focused);
            float anchorX = isFocusedSelection ? 0.5f : viewportPosition.x;
            float anchorY = isFocusedSelection ? 0.5f : viewportPosition.y;
            selectionReticle.style.left = anchorX * panelWidth - size * 0.5f;
            selectionReticle.style.top =
                (1f - anchorY) * panelHeight - size * 0.5f;
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

        private void RefreshCinematicTour()
        {
            if (!IsInitialized || tourPanel == null)
            {
                return;
            }

            bool visible = cinematicTour.IsActive;
            IsCinematicTourVisible = visible;
            tourPanel.EnableInClassList("is-hidden", !visible);
            hudRoot.EnableInClassList("tour-active", visible);
            if (!visible)
            {
                return;
            }

            CinematicTourChapter chapter = cinematicTour.CurrentChapter;
            tourProgress.text =
                $"CINEMATIC TOUR / CHAPTER {cinematicTour.CurrentChapterNumber} " +
                $"OF {cinematicTour.ChapterCount}";
            tourTitle.text = chapter.Title.ToUpperInvariant();
            tourSubtitle.text = chapter.Subtitle.ToUpperInvariant();
            tourDescription.text = chapter.Description;
            tourNextButton.text =
                cinematicTour.CurrentChapterNumber == cinematicTour.ChapterCount
                    ? "T  FINISH"
                    : "T  NEXT";
            tourMotionButton.text = motionPreference.IsReducedMotion
                ? "M  MOTION / REDUCED"
                : "M  MOTION / FULL";
        }

        private void OnTourNextClicked()
        {
            tourController.StartOrAdvance();
        }

        private void OnTourExitClicked()
        {
            tourController.Cancel();
        }

        private void OnTourMotionClicked()
        {
            tourController.ToggleReducedMotion();
        }

        private void RefreshMenu()
        {
            if (!IsInitialized || menuOverlay == null || menu == null)
            {
                return;
            }

            bool visible = menu.IsOpen;
            menuOverlay.EnableInClassList("is-hidden", !visible);
            hudRoot.EnableInClassList("menu-active", visible);
            if (!visible)
            {
                return;
            }

            ExplorerMenuPage page = menu.ActivePage;
            menuTitle.text = page switch
            {
                ExplorerMenuPage.Help => "HELP & ORIENTATION",
                ExplorerMenuPage.Settings => "SETTINGS",
                ExplorerMenuPage.CreditsAndSources => "CREDITS & SOURCES",
                _ => throw new InvalidOperationException(
                    $"Unsupported Explorer Menu page '{page}'.")
            };
            helpPage.EnableInClassList(
                "is-hidden",
                page != ExplorerMenuPage.Help);
            settingsPage.EnableInClassList(
                "is-hidden",
                page != ExplorerMenuPage.Settings);
            creditsPage.EnableInClassList(
                "is-hidden",
                page != ExplorerMenuPage.CreditsAndSources);
            menuHelpTab.EnableInClassList(
                "is-active",
                page == ExplorerMenuPage.Help);
            menuSettingsTab.EnableInClassList(
                "is-active",
                page == ExplorerMenuPage.Settings);
            menuCreditsTab.EnableInClassList(
                "is-active",
                page == ExplorerMenuPage.CreditsAndSources);
            onboardingLabel.EnableInClassList(
                "is-hidden",
                settings.Current.HasCompletedOnboarding);
            menuCloseButton.Focus();
        }

        private void RefreshSettings()
        {
            if (!IsInitialized || settings == null || masterVolumeSlider == null)
            {
                return;
            }

            ExplorerSettingsSnapshot current = settings.Current;
            masterVolumeSlider.SetValueWithoutNotify(current.MasterVolume);
            musicVolumeSlider.SetValueWithoutNotify(current.MusicVolume);
            uiVolumeSlider.SetValueWithoutNotify(current.UiVolume);
            celestialVolumeSlider.SetValueWithoutNotify(current.CelestialVolume);
            masterVolumeValue.text =
                current.MasterVolume.ToString("P0", CultureInfo.InvariantCulture);
            musicVolumeValue.text =
                current.MusicVolume.ToString("P0", CultureInfo.InvariantCulture);
            uiVolumeValue.text =
                current.UiVolume.ToString("P0", CultureInfo.InvariantCulture);
            celestialVolumeValue.text =
                current.CelestialVolume.ToString("P0", CultureInfo.InvariantCulture);
            muteToggle.SetValueWithoutNotify(current.IsMuted);
            reducedMotionToggle.SetValueWithoutNotify(
                current.MotionMode == PresentationMotionMode.ReducedMotion);
            orbitGuidesToggle.SetValueWithoutNotify(
                current.AreOrbitGuidesEnabled);
            worldLabelsToggle.SetValueWithoutNotify(
                current.AreWorldLabelsEnabled);
        }

        private void OnMenuButtonClicked()
        {
            if (menu.IsOpen)
            {
                menuController.Close();
            }
            else
            {
                menuController.Open(ExplorerMenuPage.Help);
            }
        }

        private void OnMenuCloseClicked() => menuController.Close();
        private void OnHelpTabClicked() =>
            menuController.SetPage(ExplorerMenuPage.Help);
        private void OnSettingsTabClicked() =>
            menuController.SetPage(ExplorerMenuPage.Settings);
        private void OnCreditsTabClicked() =>
            menuController.SetPage(ExplorerMenuPage.CreditsAndSources);
        private void OnRestoreDefaultsClicked() => settingsController.ResetToDefaults();

        private void OnMasterVolumeChanged(ChangeEvent<float> evt) =>
            settingsController.SetMasterVolume(evt.newValue);

        private void OnMusicVolumeChanged(ChangeEvent<float> evt) =>
            settingsController.SetMusicVolume(evt.newValue);

        private void OnUiVolumeChanged(ChangeEvent<float> evt) =>
            settingsController.SetUiVolume(evt.newValue);

        private void OnCelestialVolumeChanged(ChangeEvent<float> evt) =>
            settingsController.SetCelestialVolume(evt.newValue);

        private void OnMuteChanged(ChangeEvent<bool> evt) =>
            settingsController.SetMuted(evt.newValue);

        private void OnReducedMotionChanged(ChangeEvent<bool> evt) =>
            settingsController.SetMotionMode(
                evt.newValue
                    ? PresentationMotionMode.ReducedMotion
                    : PresentationMotionMode.FullMotion);

        private void OnOrbitGuidesChanged(ChangeEvent<bool> evt) =>
            settingsController.SetOrbitGuidesEnabled(evt.newValue);

        private void OnWorldLabelsChanged(ChangeEvent<bool> evt) =>
            settingsController.SetWorldLabelsEnabled(evt.newValue);

        private static Label RequireLabel(VisualElement root, string name)
        {
            Label label = root.Q<Label>(name);
            return label != null
                ? label
                : throw new InvalidOperationException($"HUD is missing label '{name}'.");
        }

        private static Button RequireButton(VisualElement root, string name)
        {
            Button button = root.Q<Button>(name);
            return button != null
                ? button
                : throw new InvalidOperationException(
                    $"HUD is missing button '{name}'.");
        }

        private static VisualElement RequireElement(VisualElement root, string name)
        {
            VisualElement element = root.Q<VisualElement>(name);
            return element != null
                ? element
                : throw new InvalidOperationException($"HUD is missing element '{name}'.");
        }

        private static ScrollView RequireScrollView(VisualElement root, string name)
        {
            ScrollView scrollView = root.Q<ScrollView>(name);
            return scrollView != null
                ? scrollView
                : throw new InvalidOperationException(
                    $"HUD is missing scroll view '{name}'.");
        }

        private static Slider RequireSlider(VisualElement root, string name)
        {
            Slider slider = root.Q<Slider>(name);
            return slider != null
                ? slider
                : throw new InvalidOperationException(
                    $"HUD is missing slider '{name}'.");
        }

        private static Toggle RequireToggle(VisualElement root, string name)
        {
            Toggle toggle = root.Q<Toggle>(name);
            return toggle != null
                ? toggle
                : throw new InvalidOperationException(
                    $"HUD is missing toggle '{name}'.");
        }
    }
}
