using UnityEngine;

namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>Reviewed numeric contract shared by Earth layer authoring and validation.</summary>
    public static class EarthLayerRenderingContract
    {
        /// <summary>Cloud-shell radius relative to Earth's mean-radius surface.</summary>
        public const float CloudShellRadiusMultiplier = 1.004f;

        /// <summary>Atmosphere-shell radius relative to Earth's mean-radius surface.</summary>
        public const float AtmosphereShellRadiusMultiplier = 1.018f;

        /// <summary>Total cloud spin relative to Earth's signed sidereal rotation.</summary>
        public const float CloudRotationMultiplier = 1.025f;

        /// <summary>Normal-map strength retained from the approved visual foundation.</summary>
        public const float NormalStrength = 0.28f;

        /// <summary>Sun-facing threshold where night emission begins fading in.</summary>
        public const float NightFadeStart = 0.02f;

        /// <summary>Sun-opposed threshold where night emission reaches full strength.</summary>
        public const float NightFadeEnd = 0.22f;

        /// <summary>Evaluates the nightside-only emission weight mirrored by the Earth shader.</summary>
        public static float EvaluateNightWeight(float normalDotSun)
        {
            float transition = Mathf.InverseLerp(
                NightFadeStart,
                NightFadeEnd,
                -normalDotSun);
            return transition * transition * (3f - (2f * transition));
        }
    }
}
