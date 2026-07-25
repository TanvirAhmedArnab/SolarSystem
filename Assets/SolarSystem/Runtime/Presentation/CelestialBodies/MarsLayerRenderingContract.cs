namespace Tanvir.SolarSystem.Presentation.CelestialBodies
{
    /// <summary>Reviewed presentation contract for Mars's rocky surface and thin limb.</summary>
    public static class MarsLayerRenderingContract
    {
        /// <summary>Presentation atmosphere radius relative to Mars's mean-radius surface.</summary>
        public const float AtmosphereShellRadiusMultiplier = 1.008f;

        /// <summary>Source-derived relief retained by the anchored rocky-surface shader.</summary>
        public const float ReliefStrength = 0.28f;

        /// <summary>Source-texel sample distance used to estimate restrained terrain relief.</summary>
        public const float ReliefSampleDistance = 1.5f;

        /// <summary>Restrained non-metallic surface specular response.</summary>
        public const float SurfaceSpecular = 0.025f;

        /// <summary>Dry rocky-surface smoothness.</summary>
        public const float SurfaceSmoothness = 0.1f;

        /// <summary>Narrow atmospheric-limb falloff power.</summary>
        public const float AtmosphereRimPower = 5.2f;

        /// <summary>Restrained transparent atmospheric-limb intensity.</summary>
        public const float AtmosphereIntensity = 0.16f;

        /// <summary>Minimum limb visibility on the Sun-opposed hemisphere.</summary>
        public const float AtmosphereNightsideVisibility = 0.025f;
    }
}
