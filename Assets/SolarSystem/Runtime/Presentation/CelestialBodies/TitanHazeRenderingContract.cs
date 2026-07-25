namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>Reviewed presentation contract for Titan's haze-dominant hero treatment.</summary>
    public static class TitanHazeRenderingContract
    {
        /// <summary>Presentation haze radius relative to Titan's physical mean radius.</summary>
        public const float AtmosphereShellRadiusMultiplier = 1.028f;

        /// <summary>Fraction of anchored source contrast retained beneath the haze.</summary>
        public const float SurfaceDetailStrength = 0.12f;

        /// <summary>Minimum surface visibility beneath the haze.</summary>
        public const float SurfaceAmbientBrightness = 0.035f;

        /// <summary>Sun-facing surface brightness beneath the haze.</summary>
        public const float SurfaceSunBrightness = 0.42f;

        /// <summary>Opaque-disk haze contribution.</summary>
        public const float HazeDiskOpacity = 0.64f;

        /// <summary>Additional limb haze contribution.</summary>
        public const float HazeRimIntensity = 0.31f;

        /// <summary>Broad limb falloff used for the dense haze silhouette.</summary>
        public const float HazeRimPower = 2.2f;

        /// <summary>Minimum haze visibility on the Sun-opposed hemisphere.</summary>
        public const float HazeNightsideVisibility = 0.16f;

        /// <summary>Restrained view-aligned forward-scattering presentation term.</summary>
        public const float HazeForwardScatter = 0.14f;

        /// <summary>Low-amplitude non-scientific haze variation.</summary>
        public const float HazeVariationStrength = 0.018f;

        /// <summary>Slow deterministic presentation cycles per signed Titan rotation.</summary>
        public const float HazeCyclesPerRotation = 0.04f;
    }
}
