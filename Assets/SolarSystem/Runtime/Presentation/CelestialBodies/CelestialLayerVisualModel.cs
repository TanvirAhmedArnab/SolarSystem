using System;

namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>Immutable presentation settings for optional celestial visual layers.</summary>
    public sealed class CelestialLayerVisualModel
    {
        /// <summary>Initializes validated visual-layer settings.</summary>
        public CelestialLayerVisualModel(
            string bodyStableId,
            float cloudShellRadiusMultiplier,
            float atmosphereShellRadiusMultiplier,
            float cloudRotationMultiplier)
        {
            if (string.IsNullOrWhiteSpace(bodyStableId))
            {
                throw new ArgumentException(
                    "A layered visual model requires a stable body ID.",
                    nameof(bodyStableId));
            }

            if (!float.IsFinite(cloudShellRadiusMultiplier) ||
                cloudShellRadiusMultiplier <= 1f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(cloudShellRadiusMultiplier),
                    "Cloud shell radius must be finite and greater than the body radius.");
            }

            if (!float.IsFinite(atmosphereShellRadiusMultiplier) ||
                atmosphereShellRadiusMultiplier <= cloudShellRadiusMultiplier)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(atmosphereShellRadiusMultiplier),
                    "Atmosphere shell radius must be finite and exceed the cloud shell radius.");
            }

            if (!float.IsFinite(cloudRotationMultiplier) ||
                cloudRotationMultiplier <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(cloudRotationMultiplier),
                    "Cloud rotation multiplier must be finite and positive.");
            }

            BodyStableId = bodyStableId.Trim();
            CloudShellRadiusMultiplier = cloudShellRadiusMultiplier;
            AtmosphereShellRadiusMultiplier = atmosphereShellRadiusMultiplier;
            CloudRotationMultiplier = cloudRotationMultiplier;
        }

        /// <summary>Gets the body that owns these layers.</summary>
        public string BodyStableId { get; }

        /// <summary>Gets the cloud-shell radius relative to the physical body mesh.</summary>
        public float CloudShellRadiusMultiplier { get; }

        /// <summary>Gets the atmosphere-shell radius relative to the physical body mesh.</summary>
        public float AtmosphereShellRadiusMultiplier { get; }

        /// <summary>Gets total cloud rotation relative to the body's sidereal spin.</summary>
        public float CloudRotationMultiplier { get; }
    }
}
