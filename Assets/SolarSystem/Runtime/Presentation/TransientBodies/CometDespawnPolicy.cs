using UnityEngine;

namespace Tanvir.SolarSystem.Presentation.TransientBodies
{
    /// <summary>Evaluates bounded lifetime and camera-range comet culling.</summary>
    public static class CometDespawnPolicy
    {
        private const float FarClipSafety = 0.92f;

        /// <summary>
        /// Returns whether a comet has expired or moved offscreen beyond the
        /// camera or authored solar-system envelope.
        /// </summary>
        public static bool ShouldDespawn(
            float ageSeconds,
            float maximumLifetimeSeconds,
            Vector3 viewportPosition,
            float cameraDistance,
            float cameraFarClip,
            float solarDistance,
            float solarDespawnRadius,
            float viewportMargin)
        {
            if (ageSeconds >= maximumLifetimeSeconds)
            {
                return true;
            }

            bool outsideViewport =
                viewportPosition.z <= 0f ||
                viewportPosition.x < -viewportMargin ||
                viewportPosition.x > 1f + viewportMargin ||
                viewportPosition.y < -viewportMargin ||
                viewportPosition.y > 1f + viewportMargin;
            if (!outsideViewport)
            {
                return false;
            }

            return cameraDistance >= cameraFarClip * FarClipSafety ||
                solarDistance >= solarDespawnRadius;
        }
    }
}
