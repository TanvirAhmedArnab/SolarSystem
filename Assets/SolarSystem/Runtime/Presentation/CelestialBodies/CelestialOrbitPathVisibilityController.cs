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
        private bool isCinematicTourSuppressed;
        private bool arePathsEnabledByUser = true;

        /// <summary>Gets whether required camera and path references are available.</summary>
        public bool IsInitialized =>
            cameraController != null && orbitPaths != null && orbitPaths.Length > 0;

        /// <summary>Gets the last visibility state applied to every orbit path.</summary>
        public bool ArePathsVisible => appliedVisibility ?? true;

        /// <summary>Gets whether the persisted player preference enables orbit guides.</summary>
        public bool ArePathsEnabledByUser => arePathsEnabledByUser;

        /// <summary>Gets whether the tour currently owns the visibility override.</summary>
        public bool IsCinematicTourSuppressed => isCinematicTourSuppressed;

        /// <summary>Raised after the player's orbit-guide preference changes.</summary>
        public event Action UserVisibilityChanged;

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

            bool visible = arePathsEnabledByUser &&
                !isCinematicTourSuppressed &&
                cameraController.Mode != SolarSystemCameraMode.FocusTransition &&
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

        /// <summary>Applies the player's persistent orbit-guide preference.</summary>
        public void SetUserVisibility(bool visible)
        {
            if (arePathsEnabledByUser == visible)
            {
                return;
            }

            arePathsEnabledByUser = visible;
            RefreshVisibility();
            UserVisibilityChanged?.Invoke();
        }

        /// <summary>Toggles the player's persistent orbit-guide preference.</summary>
        public void ToggleUserVisibility()
        {
            SetUserVisibility(!arePathsEnabledByUser);
        }

        /// <summary>Suppresses every orbit guide for an active cinematic shot.</summary>
        public void BeginCinematicTourSuppression()
        {
            isCinematicTourSuppressed = true;
            RefreshVisibility();
        }

        /// <summary>Releases the tour override and reapplies the camera-mode policy.</summary>
        public void EndCinematicTourSuppression()
        {
            isCinematicTourSuppressed = false;
            RefreshVisibility();
        }
    }
}
