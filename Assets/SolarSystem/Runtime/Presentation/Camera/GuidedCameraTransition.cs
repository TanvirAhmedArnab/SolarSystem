using System;
using UnityEngine;

namespace Tanvir.SolarSystem.Presentation.Camera
{
    /// <summary>Available deterministic easing curves for guided camera movement.</summary>
    public enum GuidedCameraEasing
    {
        SmoothStep = 0,
        SmootherStep = 1
    }

    /// <summary>Immutable timing and easing data for one guided camera transition.</summary>
    public readonly struct GuidedCameraTransition
    {
        /// <summary>Creates a validated transition contract.</summary>
        public GuidedCameraTransition(float durationSeconds, GuidedCameraEasing easing)
        {
            if (!float.IsFinite(durationSeconds) || durationSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(durationSeconds));
            }

            if (easing != GuidedCameraEasing.SmoothStep &&
                easing != GuidedCameraEasing.SmootherStep)
            {
                throw new ArgumentOutOfRangeException(nameof(easing));
            }

            DurationSeconds = durationSeconds;
            Easing = easing;
        }

        public float DurationSeconds { get; }
        public GuidedCameraEasing Easing { get; }
        public bool IsInstant => DurationSeconds <= 0.0001f;

        /// <summary>Gets the existing guided-presentation transition language.</summary>
        public static GuidedCameraTransition Default =>
            new GuidedCameraTransition(0.8f, GuidedCameraEasing.SmoothStep);

        /// <summary>Gets an accessibility-safe instant transition.</summary>
        public static GuidedCameraTransition Instant =>
            new GuidedCameraTransition(0f, GuidedCameraEasing.SmoothStep);

        /// <summary>Evaluates the selected deterministic easing curve.</summary>
        public float Evaluate(float progress)
        {
            float value = Mathf.Clamp01(progress);
            return Easing switch
            {
                GuidedCameraEasing.SmoothStep =>
                    value * value * (3f - (2f * value)),
                GuidedCameraEasing.SmootherStep =>
                    value * value * value *
                    (value * ((6f * value) - 15f) + 10f),
                _ => throw new InvalidOperationException(
                    $"Unsupported guided camera easing '{Easing}'.")
            };
        }
    }
}
