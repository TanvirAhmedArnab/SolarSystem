using System;
using Tanvir.SolarSystem.Presentation.Camera;
using UnityEngine;

namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>Suppresses overview orbit paths while the camera frames one body closely.</summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(100)]
    public sealed class CelestialOrbitPathVisibilityController : MonoBehaviour
    {
        [SerializeField] private SolarSystemCameraController cameraController;
        [SerializeField] private CelestialOrbitPathView[] orbitPaths =
            Array.Empty<CelestialOrbitPathView>();
        private bool? appliedVisibility;

        /// <summary>Gets whether required camera and path references are available.</summary>
        public bool IsInitialized =>
            cameraController != null && orbitPaths != null && orbitPaths.Length > 0;

        /// <summary>Gets the last visibility state applied to every orbit path.</summary>
        public bool ArePathsVisible => appliedVisibility ?? true;

        private void OnEnable()
        {
            appliedVisibility = null;
            RefreshVisibility();
        }

        private void LateUpdate()
        {
            RefreshVisibility();
        }

        /// <summary>Applies the camera-mode visibility policy immediately.</summary>
        public void RefreshVisibility()
        {
            if (!IsInitialized)
            {
                return;
            }

            bool visible = cameraController.Mode != SolarSystemCameraMode.FocusTransition &&
                cameraController.Mode != SolarSystemCameraMode.Focused;
            if (appliedVisibility == visible)
            {
                return;
            }

            for (int index = 0; index < orbitPaths.Length; index++)
            {
                CelestialOrbitPathView path = orbitPaths[index];
                if (path != null)
                {
                    path.SetPresentationVisible(visible);
                }
            }

            appliedVisibility = visible;
        }
    }
}
