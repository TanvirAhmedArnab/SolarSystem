namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>
    /// Reviewed presentation constants for airless Mercury and Moon surfaces.
    /// </summary>
    /// <remarks>
    /// Relief is estimated from the luminance of each anchored source texture. It is
    /// a restrained readability treatment, not a scientific elevation model.
    /// </remarks>
    public static class AirlessRockyVisualRenderingContract
    {
        /// <summary>Mercury source-derived relief strength.</summary>
        public const float MercuryReliefStrength = 0.24f;

        /// <summary>Mercury source-texel sample distance.</summary>
        public const float MercuryReliefSampleDistance = 1.25f;

        /// <summary>Mercury non-metallic specular response.</summary>
        public const float MercurySurfaceSpecular = 0.018f;

        /// <summary>Mercury dry-surface smoothness.</summary>
        public const float MercurySurfaceSmoothness = 0.07f;

        /// <summary>Mercury source-color visibility on its unlit hemisphere.</summary>
        public const float MercuryNightsideReadability = 0.018f;

        /// <summary>Moon source-derived relief strength.</summary>
        public const float MoonReliefStrength = 0.34f;

        /// <summary>Moon source-texel sample distance.</summary>
        public const float MoonReliefSampleDistance = 1.5f;

        /// <summary>Moon non-metallic specular response.</summary>
        public const float MoonSurfaceSpecular = 0.015f;

        /// <summary>Moon dry-regolith smoothness.</summary>
        public const float MoonSurfaceSmoothness = 0.055f;

        /// <summary>Moon source-color visibility on its unlit hemisphere.</summary>
        public const float MoonNightsideReadability = 0.022f;
    }
}
