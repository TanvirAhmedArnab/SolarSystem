using System;
using Tanvir.SolarSystem.Audio;
using Tanvir.SolarSystem.Input;
using Tanvir.SolarSystem.Interaction;
using Tanvir.SolarSystem.Presentation.Camera;
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
        [SerializeField] private SolarSystemHudPresenter hudPresenter;
        [SerializeField] private AudioDirector audioDirector;

        /// <summary>Gets the selection controller after bootstrap.</summary>
        public CelestialSelectionController SelectionController => selectionController;

        /// <summary>Gets the explorer camera controller after bootstrap.</summary>
        public SolarSystemCameraController CameraController => cameraController;

        /// <summary>Gets the active simulation-time command service.</summary>
        public SimulationTimeControlService TimeControls => timeInputController?.Service;

        /// <summary>Gets the runtime HUD presenter.</summary>
        public SolarSystemHudPresenter HudPresenter => hudPresenter;

        /// <summary>Gets the runtime audio director after bootstrap.</summary>
        public AudioDirector AudioDirector => audioDirector;

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
            hudPresenter != null &&
            hudPresenter.IsInitialized &&
            audioDirector != null &&
            audioDirector.IsInitialized;

        private void Awake()
        {
            RebuildInteractionGraph();
        }

        /// <summary>Validates serialized dependencies and builds the interaction graph.</summary>
        [ContextMenu("Rebuild Interaction Graph")]
        public void RebuildInteractionGraph()
        {
            if (inputActions == null ||
                explorerCamera == null ||
                inputAdapter == null ||
                selectionController == null ||
                cameraController == null ||
                simulationController == null ||
                timeInputController == null ||
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
            hudPresenter.Initialize(timeControls, selectionController, explorerCamera);
            audioDirector.Initialize(selectionService, timeControls, cameraController);
        }
    }
}
