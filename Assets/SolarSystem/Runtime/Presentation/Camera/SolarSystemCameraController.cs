using System;
using Tanvir.SolarSystem.Input;
using Tanvir.SolarSystem.Interaction;
using Tanvir.SolarSystem.Presentation.CelestialBodies;
using UnityEngine;

namespace Tanvir.SolarSystem.Presentation.Camera
{
    /// <summary>Runs damped free flight and interruptible celestial focus behavior.</summary>
    [DisallowMultipleComponent]
    public sealed class SolarSystemCameraController : MonoBehaviour
    {
        private const float BaseSpeed = 14f;
        private const float BoostMultiplier = 4f;
        private const float Acceleration = 36f;
        private const float LookSensitivity = 0.08f;
        private const float FocusDuration = 0.65f;
        private const float FocusDistanceMultiplier = 3.5f;
        private const float MinimumFocusDistance = 1.5f;
        private const float ZoomSensitivity = 0.004f;

        private SolarSystemInputAdapter input;
        private CelestialSelectionController selection;
        private Vector3 velocity;
        private Vector3 transitionStart;
        private Vector3 focusDirection;
        private float focusDistance;
        private float transitionElapsed;
        private float yaw;
        private float pitch;

        /// <summary>Gets the current camera interaction state.</summary>
        public SolarSystemCameraMode Mode { get; private set; }

        /// <summary>Gets the current focus target, including during transition.</summary>
        public CelestialBodyView FocusedTarget { get; private set; }

        /// <summary>Gets whether dependencies have been assigned.</summary>
        public bool IsInitialized => input != null && selection != null;

        /// <summary>Initializes the camera and subscribes to discrete interaction intent.</summary>
        public void Initialize(
            SolarSystemInputAdapter inputAdapter,
            CelestialSelectionController selectionController)
        {
            if (input != null)
            {
                input.FocusPerformed -= OnFocusPerformed;
                input.CancelPerformed -= ReturnToFreeFlight;
            }

            input = inputAdapter != null
                ? inputAdapter
                : throw new ArgumentNullException(nameof(inputAdapter));
            selection = selectionController != null
                ? selectionController
                : throw new ArgumentNullException(nameof(selectionController));
            input.FocusPerformed += OnFocusPerformed;
            input.CancelPerformed += ReturnToFreeFlight;
            Vector3 euler = transform.eulerAngles;
            yaw = euler.y;
            pitch = NormalizePitch(euler.x);
            Mode = SolarSystemCameraMode.FreeFlight;
        }

        /// <summary>Starts or redirects a smooth transition to a celestial body.</summary>
        public void Focus(CelestialBodyView target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            FocusedTarget = target;
            transitionStart = transform.position;
            focusDirection = (transform.position - target.transform.position).normalized;
            if (focusDirection.sqrMagnitude < 0.5f)
            {
                focusDirection = -target.transform.forward;
            }

            focusDistance = Mathf.Max(
                MinimumFocusDistance,
                target.CurrentDisplayRadius * FocusDistanceMultiplier);
            transitionElapsed = 0f;
            velocity = Vector3.zero;
            Mode = SolarSystemCameraMode.FocusTransition;
        }

        /// <summary>Cancels focus without snapping or changing the current camera pose.</summary>
        public void ReturnToFreeFlight()
        {
            FocusedTarget = null;
            velocity = Vector3.zero;
            Vector3 euler = transform.eulerAngles;
            yaw = euler.y;
            pitch = NormalizePitch(euler.x);
            Mode = SolarSystemCameraMode.FreeFlight;
        }

        /// <summary>Advances free flight with explicit values for deterministic validation.</summary>
        public void StepFreeFlight(
            Vector2 movement,
            float elevation,
            Vector2 lookDelta,
            bool boost,
            float unscaledDeltaTime)
        {
            if (unscaledDeltaTime <= 0f)
            {
                return;
            }

            yaw += lookDelta.x * LookSensitivity;
            pitch = Mathf.Clamp(pitch - (lookDelta.y * LookSensitivity), -85f, 85f);
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

            Vector3 desiredDirection =
                (transform.right * movement.x) +
                (transform.forward * movement.y) +
                (Vector3.up * elevation);
            desiredDirection = Vector3.ClampMagnitude(desiredDirection, 1f);
            float speed = BaseSpeed * (boost ? BoostMultiplier : 1f);
            velocity = Vector3.MoveTowards(
                velocity,
                desiredDirection * speed,
                Acceleration * unscaledDeltaTime);
            transform.position += velocity * unscaledDeltaTime;
        }

        private void LateUpdate()
        {
            if (!IsInitialized)
            {
                return;
            }

            float deltaTime = Time.unscaledDeltaTime;
            if (Mode == SolarSystemCameraMode.FreeFlight)
            {
                StepFreeFlight(
                    input.Move,
                    input.Elevate,
                    input.IsLookActive ? input.LookDelta : Vector2.zero,
                    input.IsBoostActive,
                    deltaTime);
                Cursor.lockState =
                    input.IsLookActive ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = !input.IsLookActive;
                return;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            StepFocus(deltaTime);
        }

        private void StepFocus(float deltaTime)
        {
            if (FocusedTarget == null)
            {
                ReturnToFreeFlight();
                return;
            }

            if (Mode == SolarSystemCameraMode.FocusTransition)
            {
                transitionElapsed += deltaTime;
                float progress = Mathf.Clamp01(transitionElapsed / FocusDuration);
                float eased = progress * progress * (3f - (2f * progress));
                Vector3 destination =
                    FocusedTarget.transform.position + (focusDirection * focusDistance);
                transform.position = Vector3.Lerp(transitionStart, destination, eased);
                LookAtFocus();
                if (progress >= 1f)
                {
                    Mode = SolarSystemCameraMode.Focused;
                }

                return;
            }

            if (input.IsLookActive)
            {
                Vector2 lookDelta = input.LookDelta;
                Quaternion orbit =
                    Quaternion.Euler(-lookDelta.y * LookSensitivity, lookDelta.x * LookSensitivity, 0f);
                focusDirection = (orbit * focusDirection).normalized;
            }

            focusDistance = Mathf.Clamp(
                focusDistance - (input.Zoom * ZoomSensitivity),
                Mathf.Max(MinimumFocusDistance, FocusedTarget.CurrentDisplayRadius * 1.5f),
                Mathf.Max(MinimumFocusDistance * 2f, FocusedTarget.CurrentDisplayRadius * 12f));
            transform.position =
                FocusedTarget.transform.position + (focusDirection * focusDistance);
            LookAtFocus();
        }

        private void LookAtFocus()
        {
            Vector3 direction = FocusedTarget.transform.position - transform.position;
            if (direction.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            }
        }

        private void OnDestroy()
        {
            if (input != null)
            {
                input.FocusPerformed -= OnFocusPerformed;
                input.CancelPerformed -= ReturnToFreeFlight;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void OnFocusPerformed()
        {
            if (selection.SelectedView != null)
            {
                Focus(selection.SelectedView);
            }
        }

        private static float NormalizePitch(float value) => value > 180f ? value - 360f : value;
    }
}
