using System;
using System.Collections.Generic;
using Tanvir.SolarSystem.Authoring;
using Tanvir.SolarSystem.Input;
using Tanvir.SolarSystem.Interaction;
using Tanvir.SolarSystem.Presentation.Camera;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
using UnityEngine;

namespace Tanvir.SolarSystem.Application
{
    /// <summary>
    /// Connects deterministic tour state to the existing camera, input, and
    /// celestial presentation graph.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CinematicTourController : MonoBehaviour
    {
        private readonly Dictionary<string, CelestialBodyView> viewsById =
            new Dictionary<string, CelestialBodyView>(StringComparer.Ordinal);

        private SolarSystemInputAdapter input;
        private CelestialSelectionController selection;
        private SimulationTimeInputController timeInput;
        private SolarSystemCameraController cameraController;
        private CelestialNavigationController navigation;
        private CelestialOrbitPathVisibilityController orbitVisibility;
        private CinematicTourBodyVisibilityController bodyVisibility;
        private PresentationMotionPreferenceService motionPreference;
        private UnityEngine.Camera explorerCamera;
        private bool navigatorWasVisible;
        private bool labelsWereEnabled;
        private bool orbitPathsWereVisible;
        private bool restorePending;

        public CinematicTourService Service { get; private set; }

        public bool IsInitialized =>
            input != null &&
            selection != null &&
            timeInput != null &&
            cameraController != null &&
            navigation != null &&
            orbitVisibility != null &&
            bodyVisibility != null &&
            motionPreference != null &&
            explorerCamera != null &&
            Service != null;

        /// <summary>Gets the shared persisted presentation-motion preference.</summary>
        public PresentationMotionPreferenceService MotionPreference =>
            motionPreference;

        public void Initialize(
            SolarSystemInputAdapter inputAdapter,
            CelestialSelectionController selectionController,
            SimulationTimeInputController simulationTimeInput,
            SolarSystemCameraController explorerCameraController,
            CelestialNavigationController navigationController,
            CelestialOrbitPathVisibilityController orbitPathVisibility,
            CinematicTourBodyVisibilityController tourBodyVisibility,
            PresentationMotionPreferenceService motionPreferenceService,
            UnityEngine.Camera camera,
            CinematicTourDefinition definition,
            CelestialBodyView[] bodyViews,
            GuidedPresentationCoordinator coordinator)
        {
            Release();
            input = inputAdapter ?? throw new ArgumentNullException(nameof(inputAdapter));
            selection = selectionController ??
                throw new ArgumentNullException(nameof(selectionController));
            timeInput = simulationTimeInput ??
                throw new ArgumentNullException(nameof(simulationTimeInput));
            cameraController = explorerCameraController ??
                throw new ArgumentNullException(nameof(explorerCameraController));
            navigation = navigationController ??
                throw new ArgumentNullException(nameof(navigationController));
            orbitVisibility = orbitPathVisibility ??
                throw new ArgumentNullException(nameof(orbitPathVisibility));
            bodyVisibility = tourBodyVisibility ??
                throw new ArgumentNullException(nameof(tourBodyVisibility));
            motionPreference = motionPreferenceService ??
                throw new ArgumentNullException(nameof(motionPreferenceService));
            explorerCamera = camera != null
                ? camera
                : throw new ArgumentNullException(nameof(camera));
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            ValidateAndIndexViews(bodyViews);
            CinematicTourChapter[] chapters = definition.CreateRuntimeChapters();
            ValidateChapterTargets(chapters);
            Service = new CinematicTourService(chapters, coordinator);
            Service.Changed += OnTourChanged;
            input.CinematicTourPerformed += StartOrAdvance;
            input.ToggleReducedMotionPerformed += ToggleReducedMotion;
        }

        public void StartOrAdvance()
        {
            if (!IsInitialized)
            {
                return;
            }

            if (Service.IsActive)
            {
                Service.Advance();
                return;
            }

            navigatorWasVisible = navigation.Service.IsNavigatorVisible;
            labelsWereEnabled = navigation.Service.AreWorldLabelsEnabled;
            orbitPathsWereVisible = orbitVisibility.ArePathsVisible;
            restorePending = false;
            Service.Start();
        }

        /// <summary>Toggles the persisted motion preference from keyboard or UI.</summary>
        public void ToggleReducedMotion()
        {
            if (motionPreference == null)
            {
                return;
            }

            motionPreference.Toggle();
            if (Service?.IsActive == true)
            {
                cameraController.SetCinematicTourPose(
                    CreatePose(Service.CurrentChapter),
                    ResolveTransition(Service.CurrentChapter));
            }
        }

        public void Cancel()
        {
            Service?.Cancel();
        }

        private void Update()
        {
            if (Service?.IsActive == true)
            {
                Service.Tick(Time.unscaledDeltaTime);
                if (Service.IsActive)
                {
                    cameraController.UpdateCinematicTourPose(
                        CreatePose(Service.CurrentChapter));
                }
            }

            if (restorePending &&
                cameraController != null &&
                !cameraController.IsGuidedPresentationActive)
            {
                RestoreExplorerInteraction();
            }
        }

        private void OnTourChanged()
        {
            if (!Service.IsActive)
            {
                restorePending = cameraController.IsCinematicTourActive;
                cameraController.EndCinematicTour(ResolveRestoreTransition());
                if (!restorePending)
                {
                    RestoreExplorerInteraction();
                }

                return;
            }

            selection.SetInteractionEnabled(false);
            timeInput.SetInteractionEnabled(false);
            navigation.SetNavigatorVisible(false);
            navigation.SetWorldLabelsEnabled(false);
            orbitVisibility.BeginCinematicTourSuppression();
            if (!bodyVisibility.IsTourOverrideActive)
            {
                bodyVisibility.BeginTour();
            }

            bodyVisibility.ShowOnly(Service.CurrentChapter.TargetIds);
            GuidedCameraPose pose = CreatePose(Service.CurrentChapter);
            GuidedCameraTransition transition =
                ResolveTransition(Service.CurrentChapter);
            if (cameraController.IsCinematicTourActive)
            {
                cameraController.SetCinematicTourPose(pose, transition);
            }
            else
            {
                cameraController.BeginCinematicTour(pose, transition);
            }

        }

        private GuidedCameraPose CreatePose(CinematicTourChapter chapter)
        {
            Vector3 center = Vector3.zero;
            for (int index = 0; index < chapter.TargetIds.Count; index++)
            {
                center += viewsById[chapter.TargetIds[index]].transform.position;
            }

            center /= chapter.TargetIds.Count;
            Vector3 framingDirection = ResolveFramingDirection(chapter, center);
            Quaternion baseRotation = Quaternion.LookRotation(
                -framingDirection,
                Vector3.up);
            Vector3 cameraRight = baseRotation * Vector3.right;
            Vector3 cameraUp = baseRotation * Vector3.up;
            float horizontalTangent = Mathf.Tan(
                Mathf.Atan(
                    Mathf.Tan(explorerCamera.fieldOfView * Mathf.Deg2Rad * 0.5f) *
                    (explorerCamera.aspect > 0f ? explorerCamera.aspect : 16f / 9f)));
            float verticalTangent =
                Mathf.Tan(explorerCamera.fieldOfView * Mathf.Deg2Rad * 0.5f);
            float distance = 1f;
            float depthExtent = 1f;
            float planarRadius = 1f;
            for (int index = 0; index < chapter.TargetIds.Count; index++)
            {
                CelestialBodyView view = viewsById[chapter.TargetIds[index]];
                Vector3 offset = view.transform.position - center;
                float displayRadius = view.CurrentDisplayRadius;
                float horizontal =
                    Mathf.Abs(Vector3.Dot(offset, cameraRight)) + displayRadius;
                float vertical =
                    Mathf.Abs(Vector3.Dot(offset, cameraUp)) + displayRadius;
                float depth = Vector3.Dot(offset, framingDirection);
                distance = Mathf.Max(
                    distance,
                    Mathf.Max(
                        horizontal / horizontalTangent,
                        vertical / verticalTangent) + depth);
                planarRadius = Mathf.Max(
                    planarRadius,
                    Mathf.Sqrt(
                        Mathf.Pow(Vector3.Dot(offset, cameraRight), 2f) +
                        Mathf.Pow(Vector3.Dot(offset, cameraUp), 2f)) +
                    displayRadius);
                depthExtent = Mathf.Max(
                    depthExtent,
                    Mathf.Abs(depth) + displayRadius);
            }

            distance = Mathf.Max(1f, distance * chapter.FramingPadding);
            Vector3 position = center + framingDirection * distance;
            Vector3 lookTarget =
                center -
                (cameraRight * chapter.ScreenOffset.x * planarRadius) -
                (cameraUp * chapter.ScreenOffset.y * planarRadius);
            float nearClip = Mathf.Max(0.01f, (distance - depthExtent) * 0.05f);
            float farClip = Mathf.Max(
                nearClip + 10f,
                distance + (depthExtent * 2f) + 10f);
            return new GuidedCameraPose(
                position,
                lookTarget,
                nearClip,
                farClip);
        }

        private void RestoreExplorerInteraction()
        {
            restorePending = false;
            orbitVisibility.EndCinematicTourSuppression();
            bodyVisibility.EndTour();
            selection.SetInteractionEnabled(true);
            timeInput.SetInteractionEnabled(true);
            navigation.SetWorldLabelsEnabled(labelsWereEnabled);
            navigation.SetNavigatorVisible(navigatorWasVisible);
            if (orbitVisibility.ArePathsVisible != orbitPathsWereVisible)
            {
                orbitVisibility.RefreshVisibility();
            }
        }

        private GuidedCameraTransition ResolveTransition(
            CinematicTourChapter chapter)
        {
            return motionPreference.IsReducedMotion
                ? GuidedCameraTransition.Instant
                : new GuidedCameraTransition(
                    chapter.TransitionDurationSeconds,
                    chapter.TransitionEasing);
        }

        private GuidedCameraTransition ResolveRestoreTransition()
        {
            return motionPreference.IsReducedMotion
                ? GuidedCameraTransition.Instant
                : new GuidedCameraTransition(
                    0.9f,
                    GuidedCameraEasing.SmootherStep);
        }

        private Vector3 ResolveFramingDirection(
            CinematicTourChapter chapter,
            Vector3 center)
        {
            if (chapter.FramingSpace == CinematicTourFramingSpace.World)
            {
                return chapter.FramingDirection;
            }

            Vector3 radial = center.sqrMagnitude > 0.001f
                ? center.normalized
                : Vector3.forward;
            if (chapter.FramingSpace ==
                CinematicTourFramingSpace.SunlitTargetAxis)
            {
                Vector3 targetAxis = FindFarthestTargetAxis(chapter);
                Vector3 sunwardInScreenPlane = Vector3.ProjectOnPlane(
                    -radial,
                    targetAxis);
                if (sunwardInScreenPlane.sqrMagnitude < 0.001f)
                {
                    sunwardInScreenPlane = Vector3.Cross(
                        targetAxis,
                        Vector3.up);
                    if (sunwardInScreenPlane.sqrMagnitude < 0.001f)
                    {
                        sunwardInScreenPlane = Vector3.Cross(
                            targetAxis,
                            Vector3.right);
                    }
                }

                sunwardInScreenPlane.Normalize();
                Vector3 orbitBias = Vector3.Cross(
                    targetAxis,
                    sunwardInScreenPlane).normalized;
                Vector3 verticalBias = Vector3.ProjectOnPlane(
                    Vector3.up,
                    targetAxis).normalized;
                Vector3 sunlitResolved =
                    (sunwardInScreenPlane * Mathf.Max(
                        0.25f,
                        Mathf.Abs(chapter.FramingDirection.z))) +
                    (orbitBias * chapter.FramingDirection.x * 0.35f) +
                    (verticalBias * chapter.FramingDirection.y * 0.25f);
                return Vector3.ProjectOnPlane(
                    sunlitResolved,
                    targetAxis).normalized;
            }

            Vector3 authoredAxis = chapter.FramingSpace ==
                CinematicTourFramingSpace.TargetAxis
                    ? FindFarthestTargetAxis(chapter)
                    : radial;
            Vector3 tangent = Vector3.Cross(Vector3.up, authoredAxis);
            if (tangent.sqrMagnitude < 0.001f)
            {
                tangent = Vector3.right;
            }

            tangent.Normalize();
            Vector3 resolved =
                (tangent * chapter.FramingDirection.x) +
                (Vector3.up * chapter.FramingDirection.y) +
                (authoredAxis * chapter.FramingDirection.z);
            return resolved.normalized;
        }

        private Vector3 FindFarthestTargetAxis(CinematicTourChapter chapter)
        {
            Vector3 axis = Vector3.forward;
            float farthestSquared = 0f;
            for (int first = 0; first < chapter.TargetIds.Count - 1; first++)
            {
                Vector3 firstPosition =
                    viewsById[chapter.TargetIds[first]].transform.position;
                for (int second = first + 1;
                     second < chapter.TargetIds.Count;
                     second++)
                {
                    Vector3 separation =
                        viewsById[chapter.TargetIds[second]].transform.position -
                        firstPosition;
                    float squared = separation.sqrMagnitude;
                    if (squared > farthestSquared)
                    {
                        farthestSquared = squared;
                        axis = separation;
                    }
                }
            }

            return farthestSquared > 0.001f
                ? axis.normalized
                : Vector3.forward;
        }

        private void ValidateAndIndexViews(CelestialBodyView[] bodyViews)
        {
            if (bodyViews == null || bodyViews.Length == 0)
            {
                throw new ArgumentException(
                    "Cinematic tour requires celestial body views.",
                    nameof(bodyViews));
            }

            viewsById.Clear();
            foreach (CelestialBodyView view in bodyViews)
            {
                if (view == null || !viewsById.TryAdd(view.StableId, view))
                {
                    throw new InvalidOperationException(
                        "Cinematic tour body views must be non-null and unique.");
                }
            }
        }

        private void ValidateChapterTargets(CinematicTourChapter[] chapters)
        {
            foreach (CinematicTourChapter chapter in chapters)
            {
                foreach (string targetId in chapter.TargetIds)
                {
                    if (!viewsById.ContainsKey(targetId))
                    {
                        throw new InvalidOperationException(
                            $"Tour chapter '{chapter.StableId}' references " +
                            $"unknown body '{targetId}'.");
                    }
                }
            }
        }

        private void OnDestroy()
        {
            Release();
        }

        private void Release()
        {
            if (input != null)
            {
                input.CinematicTourPerformed -= StartOrAdvance;
                input.ToggleReducedMotionPerformed -= ToggleReducedMotion;
            }

            if (Service != null)
            {
                Service.Changed -= OnTourChanged;
                Service.Cancel();
            }

            input = null;
            selection = null;
            timeInput = null;
            cameraController = null;
            navigation = null;
            orbitVisibility?.EndCinematicTourSuppression();
            orbitVisibility = null;
            bodyVisibility?.EndTour();
            bodyVisibility = null;
            motionPreference = null;
            explorerCamera = null;
            Service = null;
            viewsById.Clear();
            restorePending = false;
        }
    }
}
