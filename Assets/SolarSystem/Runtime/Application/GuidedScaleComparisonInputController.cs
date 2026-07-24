using System;
using Tanvir.SolarSystem.Input;
using Tanvir.SolarSystem.Interaction;
using Tanvir.SolarSystem.Presentation.Camera;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
using UnityEngine;

namespace Tanvir.SolarSystem.Application
{
    /// <summary>
    /// Routes comparison input and coordinates safe camera and interaction
    /// ownership around the pure guided-comparison state service.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GuidedScaleComparisonInputController : MonoBehaviour
    {
        private static readonly Vector3 FramingDirection =
            new Vector3(0.18f, 0.34f, -1f).normalized;

        private SolarSystemInputAdapter input;
        private SolarSystemSimulationController simulation;
        private SimulationTimeInputController timeInput;
        private CelestialSelectionController selection;
        private SolarSystemCameraController cameraController;
        private UnityEngine.Camera explorerCamera;
        private bool restorePending;

        /// <summary>Gets the active comparison state service.</summary>
        public GuidedScaleComparisonService Service { get; private set; }

        /// <summary>Gets whether all interaction dependencies are connected.</summary>
        public bool IsInitialized =>
            input != null &&
            simulation != null &&
            timeInput != null &&
            selection != null &&
            cameraController != null &&
            explorerCamera != null &&
            Service != null;

        /// <summary>Initializes routing and replaces any previous subscriptions.</summary>
        public void Initialize(
            SolarSystemInputAdapter inputAdapter,
            SolarSystemSimulationController simulationController,
            SimulationTimeInputController simulationTimeInput,
            CelestialSelectionController selectionController,
            SolarSystemCameraController explorerCameraController,
            UnityEngine.Camera camera,
            GuidedScaleComparisonService service)
        {
            Release();
            input = inputAdapter ??
                throw new ArgumentNullException(nameof(inputAdapter));
            simulation = simulationController ??
                throw new ArgumentNullException(nameof(simulationController));
            timeInput = simulationTimeInput ??
                throw new ArgumentNullException(nameof(simulationTimeInput));
            selection = selectionController ??
                throw new ArgumentNullException(nameof(selectionController));
            cameraController = explorerCameraController ??
                throw new ArgumentNullException(nameof(explorerCameraController));
            explorerCamera = camera != null
                ? camera
                : throw new ArgumentNullException(nameof(camera));
            Service = service ?? throw new ArgumentNullException(nameof(service));

            input.ScaleComparisonPerformed += Service.Advance;
            input.CancelPerformed += OnCancelPerformed;
            Service.Changed += OnComparisonChanged;
        }

        private void Update()
        {
            if (restorePending &&
                cameraController != null &&
                !cameraController.IsGuidedComparisonActive)
            {
                restorePending = false;
                selection.SetInteractionEnabled(true);
                timeInput.SetInteractionEnabled(true);
            }
        }

        private void OnComparisonChanged()
        {
            if (!Service.IsActive)
            {
                restorePending = cameraController.IsGuidedComparisonActive;
                cameraController.EndGuidedComparison();
                if (!restorePending)
                {
                    selection.SetInteractionEnabled(true);
                    timeInput.SetInteractionEnabled(true);
                }

                return;
            }

            restorePending = false;
            selection.SetInteractionEnabled(false);
            timeInput.SetInteractionEnabled(false);
            GuidedCameraPose pose = Service.Stage switch
            {
                GuidedScaleComparisonStage.ReadableOverview =>
                    CreateFullSystemPose(),
                GuidedScaleComparisonStage.NormalizedOrbits =>
                    CreateFullSystemPose(),
                GuidedScaleComparisonStage.LiteralEarthReference =>
                    CreateSunEarthPose(),
                _ => throw new InvalidOperationException(
                    $"Unsupported comparison stage '{Service.Stage}'.")
            };

            if (cameraController.IsGuidedComparisonActive)
            {
                cameraController.SetGuidedComparisonPose(pose);
            }
            else
            {
                cameraController.BeginGuidedComparison(pose);
            }
        }

        private GuidedCameraPose CreateFullSystemPose()
        {
            float radius = Mathf.Max(1f, simulation.GetMaximumPresentationExtent());
            return FrameSphere(Vector3.zero, radius);
        }

        private GuidedCameraPose CreateSunEarthPose()
        {
            if (!simulation.TryGetView("sun", out CelestialBodyView sun) ||
                !simulation.TryGetView("earth", out CelestialBodyView earth))
            {
                throw new InvalidOperationException(
                    "Literal comparison requires registered Sun and Earth views.");
            }

            Vector3 center = (sun.transform.position + earth.transform.position) * 0.5f;
            float radius =
                Vector3.Distance(sun.transform.position, earth.transform.position) * 0.5f +
                Mathf.Max(sun.CurrentDisplayRadius, earth.CurrentDisplayRadius);
            return FrameSphere(center, Mathf.Max(1f, radius));
        }

        private GuidedCameraPose FrameSphere(Vector3 center, float radius)
        {
            float aspect = explorerCamera.aspect > 0f
                ? explorerCamera.aspect
                : 16f / 9f;
            float verticalHalfAngle =
                explorerCamera.fieldOfView * Mathf.Deg2Rad * 0.5f;
            float horizontalHalfAngle =
                Mathf.Atan(Mathf.Tan(verticalHalfAngle) * aspect);
            float limitingHalfAngle = Mathf.Min(verticalHalfAngle, horizontalHalfAngle);
            float distance = radius / Mathf.Tan(limitingHalfAngle) * 1.16f;
            float nearClip = Mathf.Max(0.01f, radius * 0.001f);
            float farClip = Mathf.Max(
                nearClip + 10f,
                distance + (radius * 2.5f));
            return new GuidedCameraPose(
                center + (FramingDirection * distance),
                center,
                nearClip,
                farClip);
        }

        private void OnCancelPerformed()
        {
            Service.Cancel();
        }

        private void OnDestroy()
        {
            Release();
        }

        private void Release()
        {
            if (input != null && Service != null)
            {
                input.ScaleComparisonPerformed -= Service.Advance;
                input.CancelPerformed -= OnCancelPerformed;
                Service.Changed -= OnComparisonChanged;
            }

            input = null;
            simulation = null;
            timeInput = null;
            selection = null;
            cameraController = null;
            explorerCamera = null;
            Service = null;
            restorePending = false;
        }
    }
}
