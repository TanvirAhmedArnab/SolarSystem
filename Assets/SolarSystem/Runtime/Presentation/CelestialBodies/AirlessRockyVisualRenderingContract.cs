namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>
    /// Reviewed presentation constants for visible airless-body surfaces.
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

        /// <summary>Io source-derived relief strength.</summary>
        public const float IoReliefStrength = 0.22f;

        /// <summary>Io source-texel sample distance.</summary>
        public const float IoReliefSampleDistance = 1.25f;

        /// <summary>Io non-metallic specular response.</summary>
        public const float IoSurfaceSpecular = 0.016f;

        /// <summary>Io dry-surface smoothness.</summary>
        public const float IoSurfaceSmoothness = 0.045f;

        /// <summary>Io source-color visibility on its unlit hemisphere.</summary>
        public const float IoNightsideReadability = 0.018f;

        /// <summary>Europa source-derived relief strength.</summary>
        public const float EuropaReliefStrength = 0.18f;

        /// <summary>Europa source-texel sample distance.</summary>
        public const float EuropaReliefSampleDistance = 1.1f;

        /// <summary>Europa non-metallic specular response.</summary>
        public const float EuropaSurfaceSpecular = 0.035f;

        /// <summary>Europa icy-surface smoothness.</summary>
        public const float EuropaSurfaceSmoothness = 0.16f;

        /// <summary>Europa source-color visibility on its unlit hemisphere.</summary>
        public const float EuropaNightsideReadability = 0.025f;

        /// <summary>Ganymede source-derived relief strength.</summary>
        public const float GanymedeReliefStrength = 0.26f;

        /// <summary>Ganymede source-texel sample distance.</summary>
        public const float GanymedeReliefSampleDistance = 1.25f;

        /// <summary>Ganymede non-metallic specular response.</summary>
        public const float GanymedeSurfaceSpecular = 0.028f;

        /// <summary>Ganymede mixed ice-and-rock surface smoothness.</summary>
        public const float GanymedeSurfaceSmoothness = 0.12f;

        /// <summary>Ganymede source-color visibility on its unlit hemisphere.</summary>
        public const float GanymedeNightsideReadability = 0.07f;

        /// <summary>Callisto source-derived relief strength.</summary>
        public const float CallistoReliefStrength = 0.3f;

        /// <summary>Callisto source-texel sample distance.</summary>
        public const float CallistoReliefSampleDistance = 1.4f;

        /// <summary>Callisto non-metallic specular response.</summary>
        public const float CallistoSurfaceSpecular = 0.014f;

        /// <summary>Callisto old cratered-surface smoothness.</summary>
        public const float CallistoSurfaceSmoothness = 0.05f;

        /// <summary>Callisto source-color visibility on its unlit hemisphere.</summary>
        public const float CallistoNightsideReadability = 0.08f;

        /// <summary>Triton source-derived relief strength.</summary>
        public const float TritonReliefStrength = 0.21f;

        /// <summary>Triton source-texel sample distance.</summary>
        public const float TritonReliefSampleDistance = 1.25f;

        /// <summary>Triton non-metallic frost-surface specular response.</summary>
        public const float TritonSurfaceSpecular = 0.03f;

        /// <summary>Triton nitrogen-frost presentation smoothness.</summary>
        public const float TritonSurfaceSmoothness = 0.18f;

        /// <summary>Triton source-color visibility on its unlit hemisphere.</summary>
        public const float TritonNightsideReadability = 0.06f;

        /// <summary>Strength of Triton's disclosed neutral unobserved-coverage fill.</summary>
        public const float TritonCoverageFallbackStrength = 0.85f;

        /// <summary>Source-luminance threshold used to identify unobserved black coverage.</summary>
        public const float TritonCoverageThreshold = 0.015f;
    }
}
