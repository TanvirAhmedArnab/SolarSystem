using System;
using UnityEngine;

namespace Tanvir.SolarSystem.Presentation.Camera
{
    /// <summary>
    /// Converts mouse-wheel input into scale-independent focused-camera distance.
    /// </summary>
    public static class FocusZoomMath
    {
        private const float ZoomExponentPerScrollUnit = 0.0015f;
        private const float MaximumAbsoluteExponent = 2f;

        /// <summary>
        /// Applies proportional zoom while preserving the supplied distance limits.
        /// </summary>
        /// <param name="currentDistance">Current camera distance from the focus target.</param>
        /// <param name="scrollDelta">
        /// Signed mouse-wheel delta. Positive values zoom in and negative values zoom out.
        /// </param>
        /// <param name="minimumDistance">Nearest permitted focus distance.</param>
        /// <param name="maximumDistance">Farthest permitted focus distance.</param>
        /// <returns>The clamped focus distance after applying the wheel delta.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when any distance is non-finite, the minimum is not positive,
        /// or the maximum is less than the minimum.
        /// </exception>
        public static float CalculateDistance(
            float currentDistance,
            float scrollDelta,
            float minimumDistance,
            float maximumDistance)
        {
            if (!float.IsFinite(currentDistance) || currentDistance <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(currentDistance),
                    currentDistance,
                    "Current focus distance must be positive and finite.");
            }

            if (!float.IsFinite(scrollDelta))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(scrollDelta),
                    scrollDelta,
                    "Scroll delta must be finite.");
            }

            if (!float.IsFinite(minimumDistance) || minimumDistance <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(minimumDistance),
                    minimumDistance,
                    "Minimum focus distance must be positive and finite.");
            }

            if (!float.IsFinite(maximumDistance) ||
                maximumDistance < minimumDistance)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maximumDistance),
                    maximumDistance,
                    "Maximum focus distance must be finite and not less than the minimum.");
            }

            float exponent = Mathf.Clamp(
                -scrollDelta * ZoomExponentPerScrollUnit,
                -MaximumAbsoluteExponent,
                MaximumAbsoluteExponent);
            float zoomedDistance = currentDistance * Mathf.Exp(exponent);
            return Mathf.Clamp(
                zoomedDistance,
                minimumDistance,
                maximumDistance);
        }
    }
}
