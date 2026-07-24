using System;
using UnityEngine;

namespace Tanvir.SolarSystem.Presentation.Camera
{
    /// <summary>Defines one validated, deterministic camera destination.</summary>
    public readonly struct GuidedCameraPose
    {
        /// <summary>Initializes a look-at pose with explicit clipping bounds.</summary>
        public GuidedCameraPose(
            Vector3 position,
            Vector3 target,
            float nearClipPlane,
            float farClipPlane)
        {
            if (!IsFinite(position) || !IsFinite(target))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(position),
                    "Camera position and target must be finite.");
            }

            Vector3 direction = target - position;
            if (direction.sqrMagnitude <= 0.000001f)
            {
                throw new ArgumentException(
                    "Camera position and target must be distinct.",
                    nameof(target));
            }

            if (!float.IsFinite(nearClipPlane) ||
                !float.IsFinite(farClipPlane) ||
                nearClipPlane <= 0f ||
                farClipPlane <= nearClipPlane)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(farClipPlane),
                    "Camera clipping planes must be finite, positive, and ordered.");
            }

            Position = position;
            Rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            NearClipPlane = nearClipPlane;
            FarClipPlane = farClipPlane;
        }

        /// <summary>Gets the world-space camera position.</summary>
        public Vector3 Position { get; }

        /// <summary>Gets the world-space camera rotation.</summary>
        public Quaternion Rotation { get; }

        /// <summary>Gets the destination near clipping plane.</summary>
        public float NearClipPlane { get; }

        /// <summary>Gets the destination far clipping plane.</summary>
        public float FarClipPlane { get; }

        private static bool IsFinite(Vector3 value) =>
            float.IsFinite(value.x) &&
            float.IsFinite(value.y) &&
            float.IsFinite(value.z);
    }
}
