using System;
using Tanvir.SolarSystem.Input;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
using Tanvir.SolarSystem.Simulation;
using UnityEngine;

namespace Tanvir.SolarSystem.Interaction
{
    /// <summary>Resolves pointer raycasts into stable celestial selection state.</summary>
    [DisallowMultipleComponent]
    public sealed class CelestialSelectionController : MonoBehaviour
    {
        private Camera selectionCamera;
        private SolarSystemInputAdapter input;
        private SelectionService service;

        /// <summary>Gets the selected scene view, or null when selection is empty.</summary>
        public CelestialBodyView SelectedView { get; private set; }

        /// <summary>Gets the selection state service.</summary>
        public SelectionService Service => service;

        /// <summary>Gets whether dependencies have been assigned.</summary>
        public bool IsInitialized => selectionCamera != null && input != null && service != null;

        /// <summary>Gets whether pointer selection commands are accepted.</summary>
        public bool IsInteractionEnabled { get; private set; } = true;

        /// <summary>Initializes raycast selection and subscribes to input intent.</summary>
        public void Initialize(
            Camera camera,
            SolarSystemInputAdapter inputAdapter,
            SelectionService selectionService)
        {
            if (input != null)
            {
                input.SelectPerformed -= OnSelectPerformed;
            }

            selectionCamera = camera != null
                ? camera
                : throw new ArgumentNullException(nameof(camera));
            input = inputAdapter != null
                ? inputAdapter
                : throw new ArgumentNullException(nameof(inputAdapter));
            service = selectionService ??
                throw new ArgumentNullException(nameof(selectionService));
            IsInteractionEnabled = true;
            input.SelectPerformed += OnSelectPerformed;
        }

        /// <summary>Enables or suppresses pointer selection without clearing state.</summary>
        public void SetInteractionEnabled(bool enabled)
        {
            IsInteractionEnabled = enabled;
        }

        /// <summary>Raycasts from a screen position and selects the first celestial body hit.</summary>
        public bool SelectAtScreenPosition(Vector2 screenPosition)
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("Selection controller is not initialized.");
            }

            Ray ray = selectionCamera.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out RaycastHit hit))
            {
                ClearSelection();
                return false;
            }

            CelestialBodyView view = hit.collider.GetComponentInParent<CelestialBodyView>();
            if (view == null)
            {
                ClearSelection();
                return false;
            }

            Select(view);
            return true;
        }

        /// <summary>Selects a known view without coupling callers to raycast input.</summary>
        public void Select(CelestialBodyView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            SelectedView = view;
            service.Select(new CelestialBodyId(view.StableId));
        }

        /// <summary>Clears the current selected view and stable ID.</summary>
        public void ClearSelection()
        {
            SelectedView = null;
            service.Clear();
        }

        private void OnDestroy()
        {
            if (input != null)
            {
                input.SelectPerformed -= OnSelectPerformed;
            }
        }

        private void OnSelectPerformed()
        {
            if (IsInteractionEnabled)
            {
                SelectAtScreenPosition(input.PointerPosition);
            }
        }
    }
}
