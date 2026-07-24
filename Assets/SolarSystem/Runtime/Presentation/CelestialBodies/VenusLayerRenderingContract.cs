namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>Reviewed presentation contract for Venus's opaque cloud deck and limb.</summary>
    public static class VenusLayerRenderingContract
    {
        /// <summary>Cloud-top radius relative to Venus's physical mean-radius surface.</summary>
        public const float CloudShellRadiusMultiplier = 1.0115f;

        /// <summary>Atmospheric-limb radius relative to Venus's physical surface.</summary>
        public const float AtmosphereShellRadiusMultiplier = 1.02f;

        /// <summary>
        /// Approximate cloud-top spin relative to Venus's signed 243.018-day rotation.
        /// </summary>
        public const float CloudRotationMultiplier = 54.004f;

        /// <summary>Source-derived cloud relief retained by the presentation shader.</summary>
        public const float CloudReliefStrength = 0.16f;

        /// <summary>Vertical source-sample distance used for restrained relief.</summary>
        public const float ReliefSampleDistance = 1.5f;

        /// <summary>Presentation fill that keeps dense clouds readable on the nightside.</summary>
        public const float CloudAmbientBrightness = 0.16f;

        /// <summary>Sun-facing cloud brightness.</summary>
        public const float CloudSunBrightness = 1.05f;

        /// <summary>Restrained cloud-deck specular response.</summary>
        public const float CloudSpecular = 0.05f;

        /// <summary>Broad cloud-deck highlight smoothness.</summary>
        public const float CloudSmoothness = 0.28f;

        /// <summary>Atmospheric-limb falloff power.</summary>
        public const float AtmosphereRimPower = 4.6f;

        /// <summary>Restrained transparent atmospheric-limb intensity.</summary>
        public const float AtmosphereIntensity = 0.24f;

        /// <summary>Minimum limb visibility on the Sun-opposed hemisphere.</summary>
        public const float AtmosphereNightsideVisibility = 0.06f;
    }
}
