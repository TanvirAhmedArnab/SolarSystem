using System;
using Tanvir.SolarSystem.Input;
using Tanvir.SolarSystem.Interaction;
using Tanvir.SolarSystem.Presentation.Camera;
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

        /// <summary>Gets the selection controller after bootstrap.</summary>
        public CelestialSelectionController SelectionController => selectionController;

        /// <summary>Gets the explorer camera controller after bootstrap.</summary>
        public SolarSystemCameraController CameraController => cameraController;

        /// <summary>Gets whether the full interaction graph initialized successfully.</summary>
        public bool IsInitialized =>
            inputAdapter != null &&
            inputAdapter.IsInitialized &&
            selectionController != null &&
            selectionController.IsInitialized &&
            cameraController != null &&
            cameraController.IsInitialized;

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
                cameraController == null)
            {
                throw new InvalidOperationException(
                    "Interaction composition root has missing serialized dependencies.");
            }

            inputAdapter.Initialize(inputActions);
            var selectionService = new SelectionService();
            selectionController.Initialize(explorerCamera, inputAdapter, selectionService);
            cameraController.Initialize(inputAdapter, selectionController);
        }
    }
}
