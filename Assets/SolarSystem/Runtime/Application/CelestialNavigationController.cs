using System;
using System.Collections.Generic;
using Tanvir.SolarSystem.Input;
using Tanvir.SolarSystem.Interaction;
using Tanvir.SolarSystem.Presentation.Camera;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
using UnityEngine;

namespace Tanvir.SolarSystem.Application
{
    /// <summary>
    /// Routes navigator input and UI actions through the existing selection and
    /// camera application boundaries.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CelestialNavigationController : MonoBehaviour
    {
        private readonly Dictionary<string, CelestialBodyView> viewsById =
            new Dictionary<string, CelestialBodyView>(StringComparer.Ordinal);

        private SolarSystemInputAdapter input;
        private CelestialSelectionController selection;
        private SolarSystemCameraController cameraController;
        private GuidedPresentationCoordinator guidedPresentation;
        private CelestialBodyView[] orderedViews = Array.Empty<CelestialBodyView>();

        /// <summary>Gets the navigator visibility service.</summary>
        public CelestialNavigationService Service { get; private set; }

        /// <summary>Gets the validated deterministic parent-first body order.</summary>
        public IReadOnlyList<CelestialBodyView> OrderedViews => orderedViews;

        /// <summary>Gets whether all dependencies and body entries are ready.</summary>
        public bool IsInitialized =>
            input != null &&
            selection != null &&
            cameraController != null &&
            guidedPresentation != null &&
            Service != null &&
            orderedViews.Length > 0;

        /// <summary>Builds the navigator routing graph and subscribes to input.</summary>
        public void Initialize(
            SolarSystemInputAdapter inputAdapter,
            CelestialSelectionController selectionController,
            SolarSystemCameraController explorerCameraController,
            GuidedPresentationCoordinator presentationCoordinator,
            CelestialBodyView[] bodyViews)
        {
            Release();
            input = inputAdapter != null
                ? inputAdapter
                : throw new ArgumentNullException(nameof(inputAdapter));
            selection = selectionController != null
                ? selectionController
                : throw new ArgumentNullException(nameof(selectionController));
            cameraController = explorerCameraController != null
                ? explorerCameraController
                : throw new ArgumentNullException(nameof(explorerCameraController));
            guidedPresentation = presentationCoordinator ??
                throw new ArgumentNullException(nameof(presentationCoordinator));
            if (bodyViews == null || bodyViews.Length == 0)
            {
                throw new ArgumentException(
                    "Navigator requires at least one celestial body view.",
                    nameof(bodyViews));
            }

            orderedViews = new CelestialBodyView[bodyViews.Length];
            Array.Copy(bodyViews, orderedViews, bodyViews.Length);
            ValidateAndIndexViews();
            Service = new CelestialNavigationService();
            input.ToggleNavigatorPerformed += OnToggleNavigator;
            input.ToggleWorldLabelsPerformed += OnToggleWorldLabels;
            guidedPresentation.Changed += OnGuidedPresentationChanged;
        }

        /// <summary>Selects and focuses a catalog body through existing services.</summary>
        public bool NavigateTo(string stableId)
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException(
                    "Celestial navigation controller is not initialized.");
            }

            if (guidedPresentation.IsActive ||
                string.IsNullOrWhiteSpace(stableId) ||
                !viewsById.TryGetValue(stableId, out CelestialBodyView view))
            {
                return false;
            }

            selection.Select(view);
            cameraController.Focus(view);
            Service.SetNavigatorVisible(false);
            return true;
        }

        /// <summary>Sets navigator visibility unless guided comparison owns the UI.</summary>
        public void SetNavigatorVisible(bool visible)
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException(
                    "Celestial navigation controller is not initialized.");
            }

            Service.SetNavigatorVisible(visible && !guidedPresentation.IsActive);
        }

        /// <summary>Sets the persistent projected-label preference.</summary>
        public void SetWorldLabelsEnabled(bool enabled)
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException(
                    "Celestial navigation controller is not initialized.");
            }

            Service.SetWorldLabelsEnabled(enabled);
        }

        private void OnDestroy()
        {
            Release();
        }

        private void OnToggleNavigator()
        {
            if (!guidedPresentation.IsActive)
            {
                Service.ToggleNavigator();
            }
        }

        private void OnToggleWorldLabels()
        {
            if (!guidedPresentation.IsActive)
            {
                Service.ToggleWorldLabels();
            }
        }

        private void OnGuidedPresentationChanged()
        {
            if (guidedPresentation.IsActive)
            {
                Service.SetNavigatorVisible(false);
            }
        }

        private void ValidateAndIndexViews()
        {
            viewsById.Clear();
            var encountered = new HashSet<string>(StringComparer.Ordinal);
            foreach (CelestialBodyView view in orderedViews)
            {
                if (view == null || view.Definition == null)
                {
                    throw new InvalidOperationException(
                        "Navigator body views and definitions cannot be null.");
                }

                string stableId = view.StableId;
                if (!viewsById.TryAdd(stableId, view))
                {
                    throw new InvalidOperationException(
                        $"Navigator contains duplicate body ID '{stableId}'.");
                }

                string parentId = view.Definition.ParentId;
                if (!string.IsNullOrEmpty(parentId) && !encountered.Contains(parentId))
                {
                    throw new InvalidOperationException(
                        $"Navigator body '{stableId}' appears before parent '{parentId}'.");
                }

                encountered.Add(stableId);
            }
        }

        private void Release()
        {
            if (input != null)
            {
                input.ToggleNavigatorPerformed -= OnToggleNavigator;
                input.ToggleWorldLabelsPerformed -= OnToggleWorldLabels;
            }

            if (guidedPresentation != null)
            {
                guidedPresentation.Changed -= OnGuidedPresentationChanged;
            }

            input = null;
            selection = null;
            cameraController = null;
            guidedPresentation = null;
            Service = null;
            orderedViews = Array.Empty<CelestialBodyView>();
            viewsById.Clear();
        }
    }
}
