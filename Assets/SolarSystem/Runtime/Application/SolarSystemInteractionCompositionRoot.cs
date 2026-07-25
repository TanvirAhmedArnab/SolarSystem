using System;
using Tanvir.SolarSystem.Audio;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Input;
using Tanvir.SolarSystem.Infrastructure.Preferences;
using Tanvir.SolarSystem.Interaction;
using Tanvir.SolarSystem.Presentation.Camera;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
using Tanvir.SolarSystem.Presentation.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tanvir.SolarSystem.Application
{
    /// <summary>Constructs the explicit Slice 3 input, selection, and camera graph.</summary>
    [DefaultExecutionOrder(-900)]
    [DisallowMultipleComponent]
    public sealed class SolarSystemInteractionCompositionRoot : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private UnityEngine.Camera explorerCamera;
        [SerializeField] private SolarSystemInputAdapter inputAdapter;
        [SerializeField] private CelestialSelectionController selectionController;
        [SerializeField] private SolarSystemCameraController cameraController;
        [SerializeField] private SolarSystemSimulationController simulationController;
        [SerializeField] private SimulationTimeInputController timeInputController;
        [SerializeField] private GuidedScaleComparisonInputController
            scaleComparisonController;
        [SerializeField] private CelestialNavigationController navigationController;
        [SerializeField] private CinematicTourController tourController;
        [SerializeField] private CinematicTourDefinition tourDefinition;
        [SerializeField] private CelestialOrbitPathVisibilityController
            orbitPathVisibility;
        [SerializeField] private CinematicTourBodyVisibilityController
            tourBodyVisibility;
        [SerializeField] private CelestialBodyView[] bodyViews =
            Array.Empty<CelestialBodyView>();
        [SerializeField] private SolarSystemHudPresenter hudPresenter;
        [SerializeField] private AudioDirector audioDirector;

        /// <summary>Gets the selection controller after bootstrap.</summary>
        public CelestialSelectionController SelectionController => selectionController;

        /// <summary>Gets the explorer camera controller after bootstrap.</summary>
        public SolarSystemCameraController CameraController => cameraController;

        /// <summary>Gets the active simulation-time command service.</summary>
        public SimulationTimeControlService TimeControls => timeInputController?.Service;

        /// <summary>Gets the active guided scale-comparison service.</summary>
        public GuidedScaleComparisonService ScaleComparison =>
            scaleComparisonController?.Service;

        /// <summary>Gets the active celestial navigation controller.</summary>
        public CelestialNavigationController Navigation => navigationController;

        /// <summary>Gets the deterministic cinematic-tour service.</summary>
        public CinematicTourService CinematicTour => tourController?.Service;

        /// <summary>Gets the persisted presentation-motion accessibility service.</summary>
        public PresentationMotionPreferenceService MotionPreference =>
            tourController?.MotionPreference;

        /// <summary>Gets the shared orbit-guide presentation policy.</summary>
        public CelestialOrbitPathVisibilityController OrbitPathVisibility =>
            orbitPathVisibility;

        /// <summary>Gets the reversible tour target-renderer spotlight.</summary>
        public CinematicTourBodyVisibilityController TourBodyVisibility =>
            tourBodyVisibility;

        /// <summary>Gets the runtime HUD presenter.</summary>
        public SolarSystemHudPresenter HudPresenter => hudPresenter;

        /// <summary>Gets the runtime audio director after bootstrap.</summary>
        public AudioDirector AudioDirector => audioDirector;

        /// <summary>Gets the unified persisted explorer-settings controller.</summary>
        public ExplorerSettingsController ExplorerSettings { get; private set; }

        /// <summary>Gets the unified Help, Settings, and Credits menu controller.</summary>
        public ExplorerMenuController ExplorerMenu { get; private set; }

        /// <summary>Gets whether the full interaction graph initialized successfully.</summary>
        public bool IsInitialized =>
            inputAdapter != null &&
            inputAdapter.IsInitialized &&
            selectionController != null &&
            selectionController.IsInitialized &&
            cameraController != null &&
            cameraController.IsInitialized &&
            timeInputController != null &&
            timeInputController.IsInitialized &&
            scaleComparisonController != null &&
            scaleComparisonController.IsInitialized &&
            navigationController != null &&
            navigationController.IsInitialized &&
            tourController != null &&
            tourController.IsInitialized &&
            orbitPathVisibility != null &&
            orbitPathVisibility.IsInitialized &&
            tourBodyVisibility != null &&
            tourBodyVisibility.IsInitialized &&
            hudPresenter != null &&
            hudPresenter.IsInitialized &&
            audioDirector != null &&
            audioDirector.IsInitialized &&
            ExplorerSettings != null &&
            ExplorerMenu != null;

        private void Awake()
        {
            RebuildInteractionGraph();
        }

        /// <summary>Validates serialized dependencies and builds the interaction graph.</summary>
        [ContextMenu("Rebuild Interaction Graph")]
        public void RebuildInteractionGraph()
        {
            ExplorerMenu?.Dispose();
            ExplorerSettings?.Dispose();
            ExplorerMenu = null;
            ExplorerSettings = null;

            if (inputActions == null ||
                explorerCamera == null ||
                inputAdapter == null ||
                selectionController == null ||
                cameraController == null ||
                simulationController == null ||
                timeInputController == null ||
                scaleComparisonController == null ||
                navigationController == null ||
                tourController == null ||
                tourDefinition == null ||
                orbitPathVisibility == null ||
                tourBodyVisibility == null ||
                bodyViews == null ||
                bodyViews.Length == 0 ||
                hudPresenter == null ||
                audioDirector == null)
            {
                throw new InvalidOperationException(
                    "Interaction composition root has missing serialized dependencies.");
            }

            inputAdapter.Initialize(inputActions);
            var selectionService = new SelectionService();
            selectionController.Initialize(explorerCamera, inputAdapter, selectionService);
            cameraController.Initialize(inputAdapter, selectionController);
            var timeControls = new SimulationTimeControlService(simulationController);
            timeInputController.Initialize(inputAdapter, timeControls);
            var guidedPresentation = new GuidedPresentationCoordinator();
            var scaleComparison = new GuidedScaleComparisonService(
                simulationController,
                timeControls,
                guidedPresentation);
            scaleComparisonController.Initialize(
                inputAdapter,
                simulationController,
                timeInputController,
                selectionController,
                cameraController,
                explorerCamera,
                scaleComparison);
            navigationController.Initialize(
                inputAdapter,
                selectionController,
                cameraController,
                guidedPresentation,
                bodyViews);
            tourBodyVisibility.Initialize(bodyViews);
            var motionPreference = new PresentationMotionPreferenceService(
                new PlayerPrefsPresentationMotionPreferenceStore());
            tourController.Initialize(
                inputAdapter,
                selectionController,
                timeInputController,
                cameraController,
                navigationController,
                orbitPathVisibility,
                tourBodyVisibility,
                motionPreference,
                explorerCamera,
                tourDefinition,
                bodyViews,
                guidedPresentation);
            audioDirector.Initialize(
                selectionService,
                timeControls,
                cameraController,
                scaleComparison);
            ExplorerSettings = new ExplorerSettingsController(
                inputAdapter,
                audioDirector,
                motionPreference,
                navigationController,
                orbitPathVisibility,
                new PlayerPrefsExplorerSettingsStore());
            ExplorerMenu = new ExplorerMenuController(
                inputAdapter,
                ExplorerSettings,
                cameraController,
                scaleComparison,
                tourController);
            hudPresenter.Initialize(
                timeControls,
                selectionController,
                explorerCamera,
                scaleComparison,
                cameraController,
                navigationController,
                tourController,
                ExplorerSettings,
                ExplorerMenu);
        }

        private void OnDestroy()
        {
            ExplorerMenu?.Dispose();
            ExplorerSettings?.Dispose();
            ExplorerMenu = null;
            ExplorerSettings = null;
        }
    }
}
