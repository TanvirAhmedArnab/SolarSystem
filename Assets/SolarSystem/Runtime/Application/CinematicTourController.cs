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
        private UnityEngine.Camera explorerCamera;
        private bool navigatorWasVisible;
        private bool labelsWereEnabled;
        private bool restorePending;

        public CinematicTourService Service { get; private set; }

        public bool IsInitialized =>
            input != null &&
            selection != null &&
            timeInput != null &&
            cameraController != null &&
            navigation != null &&
            explorerCamera != null &&
            Service != null;

        public void Initialize(
            SolarSystemInputAdapter inputAdapter,
            CelestialSelectionController selectionController,
            SimulationTimeInputController simulationTimeInput,
            SolarSystemCameraController explorerCameraController,
            CelestialNavigationController navigationController,
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
            input.CancelPerformed += Cancel;
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
            restorePending = false;
            Service.Start();
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
                cameraController.EndCinematicTour();
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
            GuidedCameraPose pose = CreatePose(Service.CurrentChapter);
            if (cameraController.IsCinematicTourActive)
            {
                cameraController.SetCinematicTourPose(pose);
            }
            else
            {
                cameraController.BeginCinematicTour(pose);
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
            float radius = 0f;
            for (int index = 0; index < chapter.TargetIds.Count; index++)
            {
                CelestialBodyView view = viewsById[chapter.TargetIds[index]];
                radius = Mathf.Max(
                    radius,
                    Vector3.Distance(center, view.transform.position) +
                    view.CurrentDisplayRadius);
            }

            radius = Mathf.Max(1f, radius * chapter.FramingPadding);
            float aspect = explorerCamera.aspect > 0f
                ? explorerCamera.aspect
                : 16f / 9f;
            float verticalHalfAngle =
                explorerCamera.fieldOfView * Mathf.Deg2Rad * 0.5f;
            float horizontalHalfAngle =
                Mathf.Atan(Mathf.Tan(verticalHalfAngle) * aspect);
            float distance =
                radius / Mathf.Tan(Mathf.Min(verticalHalfAngle, horizontalHalfAngle));
            float nearClip = Mathf.Max(0.01f, radius * 0.001f);
            float farClip = Mathf.Max(nearClip + 10f, distance + radius * 2.5f);
            return new GuidedCameraPose(
                center + chapter.FramingDirection * distance,
                center,
                nearClip,
                farClip);
        }

        private void RestoreExplorerInteraction()
        {
            restorePending = false;
            selection.SetInteractionEnabled(true);
            timeInput.SetInteractionEnabled(true);
            navigation.SetWorldLabelsEnabled(labelsWereEnabled);
            navigation.SetNavigatorVisible(navigatorWasVisible);
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
                input.CancelPerformed -= Cancel;
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
            explorerCamera = null;
            Service = null;
            viewsById.Clear();
            restorePending = false;
        }
    }
}
